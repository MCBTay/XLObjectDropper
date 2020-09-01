using Battlehub.RTCommon;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles
{
    /// <summary>
    /// Attach to camera
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [Obsolete("Use SceneGrid instead")]
    public class RuntimeGrid : RTEComponent
    {
        public RuntimeHandlesComponent Appearance;
        public float CamOffset = 0.0f;
        public bool AutoCamOffset = true;
        public Vector3 GridOffset;

        private Camera m_camera;

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            m_camera = GetComponent<Camera>();
            #if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            #endif
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
            #if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            #endif
        }

        #if UNITY_2019_1_OR_NEWER
        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            GL.PushMatrix();
            try
            {
                GL.LoadProjectionMatrix(GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * camera.worldToCameraMatrix);

                if (camera == m_camera)
                {
                    DrawGrid(camera);
                }
            }
            finally { GL.PopMatrix(); }
        }
        #endif

        protected virtual void Start()
        {
            RuntimeHandlesComponent.InitializeIfRequired(ref Appearance);
        }

        private void OnPostRender()
        {
            Camera camera = Camera.current;
            DrawGrid(camera);
        }

        private void DrawGrid(Camera camera)
        {
            if (AutoCamOffset)
            {
                if(camera.orthographic)
                {
                    Appearance.DrawGrid(camera, GridOffset, camera.orthographicSize);
                }
                else
                {
                    Appearance.DrawGrid(camera, GridOffset, camera.transform.position.y);
                }
            }
            else
            {
                Appearance.DrawGrid(camera, GridOffset, CamOffset);
            }
        }
    }
}

