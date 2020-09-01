using Battlehub.RTCommon;
using Battlehub.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles
{
    public class SceneGrid : RTEComponent
    {
        public RuntimeHandlesComponent Appearance;

        private Mesh m_grid0Mesh;
        private Mesh m_grid1Mesh;
        private Material m_grid0Material;
        private Material m_grid1Material;

        [SerializeField]
        private Vector3 m_gridOffset = new Vector3(0f, 0.01f, 0f);

        private CommandBuffer m_commandBuffer;

        private float m_gridSize = 0.5f;
        public float SizeOfGrid
        {
            get { return m_gridSize; }
            set
            {
                if (m_gridSize != value)
                {
                    m_gridSize = value;
                    Rebuild();
                }
            }
        }

        private bool m_zTest = true;
        public bool ZTest
        {
            get { return m_zTest; }
            set
            {
                if(m_zTest != value)
                {
                    m_zTest = value;
                    Rebuild();
                }
            }
        }

        [SerializeField]
        private float m_alpha = 1.0f;
        public float Alpha
        {
            get { return m_alpha; }
            set { m_alpha = Mathf.Clamp01(value); }
        }

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            RuntimeHandlesComponent.InitializeIfRequired(ref Appearance);

            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.name = "SceneGrid Command Buffer";
            if(RenderPipelineInfo.Type == RPType.Standard)
            {
                Window.Camera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_commandBuffer);
            }
            else
            {
                Window.Camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, m_commandBuffer);
            }
            
            Rebuild();
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            m_commandBuffer.Clear();
        }

        protected override void OnDestroyOverride()
        {
            if (Window != null && Window.Camera != null && m_commandBuffer != null)
            {
                if (RenderPipelineInfo.Type == RPType.Standard)
                {
                    Window.Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_commandBuffer);
                }
                else
                {
                    Window.Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_commandBuffer);
                }
            }
            

            base.OnDestroyOverride();
            Cleanup();
        }

        private void Cleanup()
        {
            if (m_grid0Material != null)
            {
                Destroy(m_grid0Material);
            }
            if (m_grid1Material != null)
            {
                Destroy(m_grid1Material);
            }
            if (m_grid0Mesh != null)
            {
                Destroy(m_grid0Mesh);
            }
            if (m_grid1Mesh != null)
            {
                Destroy(m_grid1Mesh);
            }
        }

        private void Rebuild()
        {
            Cleanup();
            if (Appearance == null)
            {
                return;
            }

            m_grid0Material = CreateGridMaterial(0.5f, m_zTest);
            m_grid1Material = CreateGridMaterial(0.5f, m_zTest);

            m_grid0Mesh = Appearance.CreateGridMesh(Appearance.Colors.GridColor, m_gridSize);
            m_grid1Mesh = Appearance.CreateGridMesh(Appearance.Colors.GridColor, m_gridSize);
        }


        protected virtual void Update()
        {
            float h = GetCameraOffset();
            h = Mathf.Abs(h);
            h = Mathf.Max(1, h);
            float scale = MathHelper.CountOfDigits(h);
            float fadeDistance = h * 10;

            float alpha0 = GetAlpha(0, h, scale);
            float alpha1 = GetAlpha(1, h, scale);

            SetGridAlpha(m_grid0Material, alpha0 * m_alpha, fadeDistance);
            SetGridAlpha(m_grid1Material, alpha1 * m_alpha, fadeDistance);

            float pow0 = Mathf.Pow(10, scale - 1);
            float pow1 = Mathf.Pow(10, scale);

            Matrix4x4 grid0 =
                transform.localToWorldMatrix * Matrix4x4.TRS(GetGridPostion(pow0), Quaternion.identity, Vector3.one * pow0);
            Matrix4x4 grid1 =
                transform.localToWorldMatrix * Matrix4x4.TRS(GetGridPostion(pow1), Quaternion.identity, Vector3.one * pow1);

            m_commandBuffer.Clear();

            if (RenderPipelineInfo.Type == RPType.Standard)
            {
                m_commandBuffer.DrawMesh(m_grid0Mesh, grid0, m_grid0Material);
                m_commandBuffer.DrawMesh(m_grid1Mesh, grid1, m_grid1Material);
            }
            else
            {
                m_commandBuffer.DrawMeshInstanced(m_grid0Mesh, 0, m_grid0Material, 0, new[] { grid0 });
                m_commandBuffer.DrawMeshInstanced(m_grid1Mesh, 0, m_grid1Material, 0, new[] { grid1 });
            }
        }

        private Vector3 GetGridPostion(float spacing)
        {
            Vector3 position = Window.Camera.transform.position;
            position = transform.InverseTransformPoint(position);

            spacing *= m_gridSize;

            position.x = Mathf.Floor(position.x / spacing) * spacing;
            position.z = Mathf.Floor(position.z / spacing) * spacing;
            position.y = 0;

            position += m_gridOffset;

            return position;
        }

        private void SetGridAlpha(Material gridMaterial, float alpha, float fadeDistance)
        {
            Color color = gridMaterial.GetColor("_GridColor");
            color.a = alpha;
            gridMaterial.SetColor("_GridColor", color);
            gridMaterial.SetFloat("_FadeDistance", fadeDistance);

            if (Window.Camera.orthographic)
            {
                gridMaterial.SetFloat("_CameraSize", Window.Camera.orthographicSize);
            }
        }

        private Material CreateGridMaterial(float scale, bool zTest)
        {
            Shader shader =  Shader.Find("Battlehub/RTHandles/Grid");
            Material material = new Material(shader);
            material.SetColor("_GridColor", Appearance.Colors.GridColor);
            material.SetFloat("_ZTest", zTest ? (float)CompareFunction.LessEqual : (float)CompareFunction.Always);
            material.enableInstancing = true;
            return material;
        }

        private float GetCameraOffset()
        {
            if (Window.Camera.orthographic)
            {
                return Window.Camera.orthographicSize;
            }

            Vector3 position = Window.Camera.transform.position;
            position = transform.InverseTransformPoint(position);
            return position.y;
        }

        private float GetAlpha(int grid, float h, float scale)
        {
            float nextSpacing = Mathf.Pow(10, scale);
            if (grid == 0)
            {
                float spacing = Mathf.Pow(10, scale - 1);
                return 1.0f - (h - spacing) / (nextSpacing - spacing);
            }

            float nextNextSpacing = Mathf.Pow(10, scale + 1);
            return (h * 10 - nextSpacing) / (nextNextSpacing - nextSpacing);
        }


    }
}

