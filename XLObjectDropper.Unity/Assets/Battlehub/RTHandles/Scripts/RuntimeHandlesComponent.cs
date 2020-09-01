using UnityEngine;
using System.Collections;
using Battlehub.RTCommon;
using Battlehub.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.RTHandles
{
    public enum RuntimeHandleAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        XY = X | Y,
        XZ = X | Z,
        YZ = Y | Z,
        Screen = 8,
        Free = 16,
        Snap = 32,
    }

    [System.Serializable]
    public class RTHColors
    {
        public Color32 DisabledColor = new Color32(128, 128, 128, 128);
        public Color32 XColor = new Color32(187, 70, 45, 255);        
        public Color32 YColor = new Color32(139, 206, 74, 255);
        public Color32 ZColor = new Color32(55, 115, 244, 255);
        public Color32 AltColor = new Color32(192, 192, 192, 224);
        public Color32 AltColor2 = new Color32(0x38, 0x38, 0x38, 224);
        public Color32 SelectionColor = new Color32(239, 238, 64, 255);
        public Color32 SelectionAltColor = new Color(0, 0, 0, 0.1f);
        public Color32 BoundsColor = Color.green;
        public Color32 GridColor = new Color(1, 1, 1, 0.1f);
    }

    public interface IRuntimeHandlesComponent
    {
        RTHColors Colors
        {
            get;
            set;
        }

        float HandleScale
        {
            get;
            set;
        }

        float SelectionMargin
        {
            get;
            set;
        }
        
        bool InvertZAxis
        {
            get;
            set;
        }
        
        bool PositionHandleArrowOnly
        {
            get;
            set;
        }

        float SceneGizmoScale
        {
            get;
            set;
        }

        void ApplySettings();
    }
    
    /// <summary>
    /// GL Drawing routines for all handles, gizmos and grid
    /// </summary>
    public class RuntimeHandlesComponent : MonoBehaviour, IRuntimeHandlesComponent
    {
        [SerializeField]
        private RTHColors m_colors = new RTHColors();
        public RTHColors Colors
        {
            get { return m_colors; }
            set { m_colors = value; }
        }

        [SerializeField]
        private float m_handleScale = 1.0f;
        public float HandleScale
        {
            get { return m_handleScale; }
            set { m_handleScale = value; }
        }

        [SerializeField]
        private float m_selectionMargin = 1;
        public float SelectionMargin
        {
            get { return m_selectionMargin * m_handleScale; }
            set { m_selectionMargin = value; }
        }

        [SerializeField]
        private bool m_invertZAxis = false;
        public bool InvertZAxis
        {
            get { return m_invertZAxis; }
            set { m_invertZAxis = value; }

        }

        [SerializeField]
        private bool m_positionHandleArrowOnly = false;
        public bool PositionHandleArrowOnly
        {
            get { return m_positionHandleArrowOnly; }
            set { m_positionHandleArrowOnly = value; }
        }

        [SerializeField]
        private float m_sceneGizmoScale = 1.0f;
        public float SceneGizmoScale
        {
            get { return m_sceneGizmoScale; }
            set { m_sceneGizmoScale = value; }
        }
        
        public Vector3 Forward
        {
            get { return m_invertZAxis ? Vector3.back : Vector3.forward; }
        }

        protected Mesh Arrows;
        protected Mesh ArrowY;
        protected Mesh ArrowX;
        protected Mesh ArrowZ;
        protected Mesh SelectionArrowY;
        protected Mesh SelectionArrowX;
        protected Mesh SelectionArrowZ;
        protected Mesh DisabledArrowY;
        protected Mesh DisabledArrowX;
        protected Mesh DisabledArrowZ;

        protected Mesh SelectionCube;
        protected Mesh DisabledCube;
        protected Mesh CubeX;
        protected Mesh CubeY;
        protected Mesh CubeZ;
        protected Mesh CubeUniform;

        protected Mesh SceneGizmoSelectedAxis;
        protected Mesh SceneGizmoXAxis;
        protected Mesh SceneGizmoYAxis;
        protected Mesh SceneGizmoZAxis;
        protected Mesh SceneGizmoCube;
        protected Mesh SceneGizmoSelectedCube;
        protected Mesh SceneGizmoQuad;

        protected Material m_shapesMaterialZTest;
        protected Material m_shapesMaterialZTest2;
        protected Material m_shapesMaterialZTest3;
        protected Material m_shapesMaterialZTest4;
        protected Material m_shapesMaterialZTestOffset;
        protected Material m_shapesMaterial;
        protected Material m_linesMaterial;
        protected Material m_linesMaterialZTest;
        protected Material m_linesClipMaterial;
        protected Material m_linesClipUsingClipPlaneMaterial;
        protected Material m_linesBillboardMaterial;
        protected Material m_xMaterial;
        protected Material m_yMaterial;
        protected Material m_zMaterial;
        protected Material m_gridMaterial;

        private static RuntimeHandlesComponent m_instance;
        public static void InitializeIfRequired(ref RuntimeHandlesComponent handles)
        {
            if (handles == null)
            {
                if(m_instance == null)
                {
                    m_instance = FindObjectOfType<RuntimeHandlesComponent>();
                    if (m_instance == null)
                    {
                        IRTE rte = IOC.Resolve<IRTE>();
                        GameObject runtimeHandles = new GameObject("RuntimeHandlesComponent");
                        runtimeHandles.transform.SetParent(rte.Root.transform, false);
                        m_instance = runtimeHandles.AddComponent<RuntimeHandlesComponent>();
                    }
                }
                handles = m_instance;
            }
        }

        private float m_oldHandleScale;
        private bool m_oldInvertZAxis;

        protected virtual void Awake()
        {
            m_oldHandleScale = m_handleScale;
            m_oldInvertZAxis = m_invertZAxis;
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            if (m_instance == this)
            {
                m_instance = null;
            }
            Cleanup();
        }

        protected virtual void Update()
        {
            if(m_oldHandleScale != m_handleScale || m_oldInvertZAxis != m_invertZAxis)
            {
                m_oldHandleScale = m_handleScale;
                m_oldInvertZAxis = m_invertZAxis;
                ApplySettings();
            }
        }

        public void ApplySettings()
        {
            Cleanup();
            Initialize();
        }

        private void Initialize()
        {
            m_linesMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
            m_linesMaterial.color = Color.white;
            m_linesMaterial.enableInstancing = true;

            m_linesMaterialZTest = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
            m_linesMaterialZTest.color = Color.white;
            m_linesMaterialZTest.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            m_linesMaterialZTest.enableInstancing = true;

            m_linesClipMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColorClip"));
            m_linesClipMaterial.color = Color.white;
            m_linesClipMaterial.enableInstancing = true;

            m_linesClipUsingClipPlaneMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColorClipUsingClipPlane"));
            m_linesClipUsingClipPlaneMaterial.color = Color.white;
            m_linesClipUsingClipPlaneMaterial.enableInstancing = true;

            m_linesBillboardMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColorBillboard"));
            m_linesBillboardMaterial.DisableKeyword("_USE_CAMERA_POSITION");
            m_linesBillboardMaterial.color = Color.white;
            m_linesBillboardMaterial.enableInstancing = true;

            m_shapesMaterial = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            m_shapesMaterial.color = Color.white;
            m_shapesMaterial.enableInstancing = true;

            m_shapesMaterialZTest = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            m_shapesMaterialZTest.color = new Color(1, 1, 1, 0);
            m_shapesMaterialZTest.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            m_shapesMaterialZTest.SetFloat("_ZWrite", 1.0f);
            m_shapesMaterialZTest.enableInstancing = true;

            m_shapesMaterialZTestOffset = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            m_shapesMaterialZTestOffset.color = new Color(1, 1, 1, 1);
            m_shapesMaterialZTestOffset.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            m_shapesMaterialZTestOffset.SetFloat("_ZWrite", 1.0f);
            m_shapesMaterialZTestOffset.SetFloat("_OFactors", -1.0f);
            m_shapesMaterialZTestOffset.SetFloat("_OUnits", -1.0f);
            m_shapesMaterialZTestOffset.enableInstancing = true;

            m_shapesMaterialZTest2 = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            m_shapesMaterialZTest2.color = new Color(1, 1, 1, 0);
            m_shapesMaterialZTest2.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            m_shapesMaterialZTest2.SetFloat("_ZWrite", 1.0f);
            m_shapesMaterialZTest2.enableInstancing = true;

            m_shapesMaterialZTest3 = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            m_shapesMaterialZTest3.color = new Color(1, 1, 1, 0);
            m_shapesMaterialZTest3.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            m_shapesMaterialZTest3.SetFloat("_ZWrite", 1.0f);
            m_shapesMaterialZTest3.enableInstancing = true;

            m_shapesMaterialZTest4 = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            m_shapesMaterialZTest4.color = new Color(1, 1, 1, 0);
            m_shapesMaterialZTest4.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            m_shapesMaterialZTest4.SetFloat("_ZWrite", 1.0f);
            m_shapesMaterialZTest4.enableInstancing = true;

            m_xMaterial = new Material(Shader.Find("Battlehub/RTCommon/Billboard"));
            m_xMaterial.color = Color.white;
            m_xMaterial.mainTexture = Resources.Load<Texture>("Battlehub.RuntimeHandles.x");
            m_xMaterial.enableInstancing = true;
            m_yMaterial = new Material(Shader.Find("Battlehub/RTCommon/Billboard"));
            m_yMaterial.color = Color.white;
            m_yMaterial.mainTexture = Resources.Load<Texture>("Battlehub.RuntimeHandles.y");
            m_yMaterial.enableInstancing = true;
            m_zMaterial = new Material(Shader.Find("Battlehub/RTCommon/Billboard"));
            m_zMaterial.color = Color.white;
            m_zMaterial.mainTexture = Resources.Load<Texture>("Battlehub.RuntimeHandles.z");
            m_zMaterial.enableInstancing = true;

            m_gridMaterial = new Material(Shader.Find("Battlehub/RTHandles/Grid"));
            m_gridMaterial.color = Colors.GridColor;
            m_gridMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Never);
            m_gridMaterial.enableInstancing = true;

            Mesh selectionArrowMesh = RuntimeGraphics.CreateConeMesh(m_colors.SelectionColor, m_handleScale);
            Mesh disableArrowMesh = RuntimeGraphics.CreateConeMesh(m_colors.DisabledColor, m_handleScale);

            CombineInstance yArrow = new CombineInstance();
            yArrow.mesh = selectionArrowMesh;
            yArrow.transform = Matrix4x4.TRS(Vector3.up * m_handleScale, Quaternion.identity, Vector3.one);
            SelectionArrowY = new Mesh();
            SelectionArrowY.CombineMeshes(new[] { yArrow }, true);
            SelectionArrowY.RecalculateNormals();

            yArrow.mesh = disableArrowMesh;
            yArrow.transform = Matrix4x4.TRS(Vector3.up * m_handleScale, Quaternion.identity, Vector3.one);
            DisabledArrowY = new Mesh();
            DisabledArrowY.CombineMeshes(new[] { yArrow }, true);
            DisabledArrowY.RecalculateNormals();

            yArrow.mesh = RuntimeGraphics.CreateConeMesh(m_colors.YColor, m_handleScale);
            yArrow.transform = Matrix4x4.TRS(Vector3.up * m_handleScale, Quaternion.identity, Vector3.one);
            ArrowY = new Mesh();
            ArrowY.CombineMeshes(new[] { yArrow }, true);
            ArrowY.RecalculateNormals();

            CombineInstance xArrow = new CombineInstance();
            xArrow.mesh = selectionArrowMesh;
            xArrow.transform = Matrix4x4.TRS(Vector3.right * m_handleScale, Quaternion.AngleAxis(-90, Vector3.forward), Vector3.one);
            SelectionArrowX = new Mesh();
            SelectionArrowX.CombineMeshes(new[] { xArrow }, true);
            SelectionArrowX.RecalculateNormals();

            xArrow.mesh = disableArrowMesh;
            xArrow.transform = Matrix4x4.TRS(Vector3.right * m_handleScale, Quaternion.AngleAxis(-90, Vector3.forward), Vector3.one);
            DisabledArrowX = new Mesh();
            DisabledArrowX.CombineMeshes(new[] { xArrow }, true);
            DisabledArrowX.RecalculateNormals();

            xArrow.mesh = RuntimeGraphics.CreateConeMesh(m_colors.XColor, m_handleScale);
            xArrow.transform = Matrix4x4.TRS(Vector3.right * m_handleScale, Quaternion.AngleAxis(-90, Vector3.forward), Vector3.one);
            ArrowX = new Mesh();
            ArrowX.CombineMeshes(new[] { xArrow }, true);
            ArrowX.RecalculateNormals();

            Vector3 zAxis = Forward * m_handleScale;
            Quaternion zRotation = m_invertZAxis ? Quaternion.AngleAxis(-90, Vector3.right) : Quaternion.AngleAxis(90, Vector3.right);
            CombineInstance zArrow = new CombineInstance();
            zArrow.mesh = selectionArrowMesh;
            zArrow.transform = Matrix4x4.TRS(zAxis, zRotation, Vector3.one);
            SelectionArrowZ = new Mesh();
            SelectionArrowZ.CombineMeshes(new[] { zArrow }, true);
            SelectionArrowZ.RecalculateNormals();

            zArrow.mesh = disableArrowMesh;
            zArrow.transform = Matrix4x4.TRS(zAxis, zRotation, Vector3.one);
            DisabledArrowZ = new Mesh();
            DisabledArrowZ.CombineMeshes(new[] { zArrow }, true);
            DisabledArrowZ.RecalculateNormals();

            zArrow.mesh = RuntimeGraphics.CreateConeMesh(m_colors.ZColor, m_handleScale);
            zArrow.transform = Matrix4x4.TRS(zAxis, zRotation, Vector3.one);
            ArrowZ = new Mesh();
            ArrowZ.CombineMeshes(new[] { zArrow }, true);
            ArrowZ.RecalculateNormals();

            yArrow.mesh = RuntimeGraphics.CreateConeMesh(m_colors.YColor, m_handleScale);
            xArrow.mesh = RuntimeGraphics.CreateConeMesh(m_colors.XColor, m_handleScale);
            zArrow.mesh = RuntimeGraphics.CreateConeMesh(m_colors.ZColor, m_handleScale);
            Arrows = new Mesh();
            Arrows.CombineMeshes(new[] { yArrow, xArrow, zArrow }, true);
            Arrows.RecalculateNormals();

            SelectionCube = RuntimeGraphics.CreateCubeMesh(m_colors.SelectionColor, Vector3.zero, m_handleScale, 0.1f, 0.1f, 0.1f);
            DisabledCube = RuntimeGraphics.CreateCubeMesh(m_colors.DisabledColor, Vector3.zero, m_handleScale, 0.1f, 0.1f, 0.1f);
            CubeX = RuntimeGraphics.CreateCubeMesh(m_colors.XColor, Vector3.zero, m_handleScale, 0.1f, 0.1f, 0.1f);
            CubeY = RuntimeGraphics.CreateCubeMesh(m_colors.YColor, Vector3.zero, m_handleScale, 0.1f, 0.1f, 0.1f);
            CubeZ = RuntimeGraphics.CreateCubeMesh(m_colors.ZColor, Vector3.zero, m_handleScale, 0.1f, 0.1f, 0.1f);
            CubeUniform = RuntimeGraphics.CreateCubeMesh(m_colors.AltColor, Vector3.zero, m_handleScale, 0.1f, 0.1f, 0.1f);

            SceneGizmoSelectedAxis = CreateSceneGizmoHalfAxis(m_colors.SelectionColor, Quaternion.AngleAxis(90, Vector3.right));
            SceneGizmoXAxis = CreateSceneGizmoAxis(m_colors.XColor, m_colors.AltColor, Quaternion.AngleAxis(-90, Vector3.forward));
            SceneGizmoYAxis = CreateSceneGizmoAxis(m_colors.YColor, m_colors.AltColor, Quaternion.identity);
            SceneGizmoZAxis = CreateSceneGizmoAxis(m_colors.ZColor, m_colors.AltColor, zRotation);
            SceneGizmoCube = RuntimeGraphics.CreateCubeMesh(m_colors.AltColor, Vector3.zero, 1);
            SceneGizmoSelectedCube = RuntimeGraphics.CreateCubeMesh(m_colors.SelectionColor, Vector3.zero, 1);
            SceneGizmoQuad = RuntimeGraphics.CreateQuadMesh();
        }


        private void Cleanup()
        {
            if(Arrows != null) Destroy(Arrows);
            if (ArrowY != null) Destroy(ArrowY);
            if (ArrowZ != null) Destroy(ArrowZ);
            if (SelectionArrowY != null) Destroy(SelectionArrowY);
            if (SelectionArrowX != null) Destroy(SelectionArrowX);
            if (SelectionArrowZ != null) Destroy(SelectionArrowZ);
            if (DisabledArrowY != null) Destroy(DisabledArrowY);
            if (DisabledArrowX != null) Destroy(DisabledArrowX);
            if (DisabledArrowZ != null) Destroy(DisabledArrowZ);

            if (SelectionCube != null) Destroy(SelectionCube);
            if (DisabledCube != null) Destroy(DisabledCube);
            if (CubeX != null) Destroy(CubeX);
            if (CubeY != null) Destroy(CubeY);
            if (CubeZ != null) Destroy(CubeZ);
            if (CubeUniform != null) Destroy(CubeUniform);

            if (SceneGizmoSelectedAxis != null) Destroy(SceneGizmoSelectedAxis);
            if (SceneGizmoXAxis != null) Destroy(SceneGizmoXAxis);
            if (SceneGizmoYAxis != null) Destroy(SceneGizmoYAxis);
            if (SceneGizmoZAxis != null) Destroy(SceneGizmoZAxis);
            if (SceneGizmoCube != null) Destroy(SceneGizmoCube);
            if (SceneGizmoSelectedCube != null) Destroy(SceneGizmoSelectedCube);
            if (SceneGizmoQuad != null) Destroy(SceneGizmoQuad);

            if (m_shapesMaterialZTest != null) Destroy(m_shapesMaterialZTest);
            if (m_shapesMaterialZTest2 != null) Destroy(m_shapesMaterialZTest2);
            if (m_shapesMaterialZTest3 != null) Destroy(m_shapesMaterialZTest3);
            if (m_shapesMaterialZTest4 != null) Destroy(m_shapesMaterialZTest4);
            if (m_shapesMaterialZTestOffset != null) Destroy(m_shapesMaterialZTestOffset);
            if (m_shapesMaterial != null) Destroy(m_shapesMaterial);
            if (m_linesMaterial != null) Destroy(m_linesMaterial);
            if (m_linesMaterialZTest != null) Destroy(m_linesMaterialZTest);
            if (m_linesClipMaterial != null) Destroy(m_linesClipMaterial);
            if (m_linesClipUsingClipPlaneMaterial != null) Destroy(m_linesClipUsingClipPlaneMaterial);
            if (m_linesBillboardMaterial != null) Destroy(m_linesBillboardMaterial);
            if (m_xMaterial != null) Destroy(m_xMaterial);
            if (m_yMaterial != null) Destroy(m_yMaterial);
            if (m_zMaterial != null) Destroy(m_zMaterial);
            if (m_gridMaterial != null) Destroy(m_gridMaterial);
        }
        
        private static Mesh CreateSceneGizmoHalfAxis(Color color, Quaternion rotation)
        {
            const float scale = 0.1f;
            Mesh cone1 = RuntimeGraphics.CreateConeMesh(color, 1);

            CombineInstance cone1Combine = new CombineInstance();
            cone1Combine.mesh = cone1;
            cone1Combine.transform = Matrix4x4.TRS(Vector3.up * scale, Quaternion.AngleAxis(180, Vector3.right), Vector3.one);

            Mesh result = new Mesh();
            result.CombineMeshes(new[] { cone1Combine }, true);

            CombineInstance rotateCombine = new CombineInstance();
            rotateCombine.mesh = result;
            rotateCombine.transform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

            result = new Mesh();
            result.CombineMeshes(new[] { rotateCombine }, true);
            result.RecalculateNormals();
            return result;
        }

        private static Mesh CreateSceneGizmoAxis(Color axisColor, Color altColor, Quaternion rotation)
        {
            const float scale = 0.1f;
            Mesh cone1 = RuntimeGraphics.CreateConeMesh(axisColor, 1);
            Mesh cone2 = RuntimeGraphics.CreateConeMesh(altColor, 1);

            CombineInstance cone1Combine = new CombineInstance();
            cone1Combine.mesh = cone1;
            cone1Combine.transform = Matrix4x4.TRS(Vector3.up * scale,  Quaternion.AngleAxis(180, Vector3.right), Vector3.one);

            CombineInstance cone2Combine = new CombineInstance();
            cone2Combine.mesh = cone2;
            cone2Combine.transform = Matrix4x4.TRS(Vector3.down * scale, Quaternion.identity, Vector3.one);

            Mesh result = new Mesh();
            result.CombineMeshes(new[] { cone1Combine, cone2Combine }, true);

            CombineInstance rotateCombine = new CombineInstance();
            rotateCombine.mesh = result;
            rotateCombine.transform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

            result = new Mesh();
            result.CombineMeshes(new[] { rotateCombine }, true);
            result.RecalculateNormals();
            return result;
        }

        public static float GetScreenScale(Vector3 position, Camera camera)
        {
            return RuntimeGraphics.GetScreenScale(position, camera);
        }

        private void DoAxes(Vector3 position, Matrix4x4 transform, RuntimeHandleAxis selectedAxis, bool xLocked, bool yLocked, bool zLocked)
        {
            Vector3 x = Vector3.right * m_handleScale;
            Vector3 y = Vector3.up * m_handleScale;
            Vector3 z = Forward * m_handleScale;

            x = transform.MultiplyVector(x);
            y = transform.MultiplyVector(y);
            z = transform.MultiplyVector(z);

            if(xLocked)
            {
                GL.Color(m_colors.DisabledColor);
            }
            else
            {
                GL.Color((selectedAxis & RuntimeHandleAxis.X) == 0 ? m_colors.XColor : m_colors.SelectionColor);
            }
            
            GL.Vertex(position);
            GL.Vertex(position + x);

            if(yLocked)
            {
                GL.Color(m_colors.DisabledColor);
            }
            else
            {
                GL.Color((selectedAxis & RuntimeHandleAxis.Y) == 0 ? m_colors.YColor : m_colors.SelectionColor);
            }
            
            GL.Vertex(position);
            GL.Vertex(position + y);

            if(zLocked)
            {
                GL.Color(m_colors.DisabledColor);
            }
            else
            {
                GL.Color((selectedAxis & RuntimeHandleAxis.Z) == 0 ? m_colors.ZColor : m_colors.SelectionColor);
            }
            
            GL.Vertex(position);
            GL.Vertex(position + z);
        }

        
        public void DoPositionHandle(Camera camera, Vector3 position, Quaternion rotation, 
            RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None, bool snapMode = false, LockObject lockObject = null)
        {
            float screenScale = GetScreenScale(position, camera);
            Matrix4x4 transform = Matrix4x4.TRS(position, rotation, new Vector3(screenScale, screenScale, screenScale));

            m_linesMaterial.SetPass(0);

            GL.Begin(GL.LINES);

            bool xLocked = lockObject != null && lockObject.PositionX;
            bool yLocked = lockObject != null && lockObject.PositionY;
            bool zLocked = lockObject != null && lockObject.PositionZ;

            DoAxes(position, transform, selectedAxis, xLocked, yLocked, zLocked);

            const float s = 0.2f;
            Vector3 x = Vector3.right * s;
            Vector3 y = Vector3.up * s;
            Vector3 z = Vector3.forward * s;

            if (snapMode)
            {
                GL.End();

                m_linesBillboardMaterial.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(transform);
                GL.Begin(GL.LINES);

                if(selectedAxis == RuntimeHandleAxis.Snap)
                {
                    GL.Color(m_colors.SelectionColor);
                }
                else
                {
                    GL.Color(m_colors.AltColor);
                }
                
                float s2 = s / 2 * m_handleScale;
                Vector3 p0 = new Vector3( s2,  s2, 0);
                Vector3 p1 = new Vector3( s2, -s2, 0);
                Vector3 p2 = new Vector3(-s2, -s2, 0);
                Vector3 p3 = new Vector3(-s2,  s2, 0);

                GL.Vertex(p0);
                GL.Vertex(p1);
                GL.Vertex(p1);
                GL.Vertex(p2);
                GL.Vertex(p2);
                GL.Vertex(p3);
                GL.Vertex(p3);
                GL.Vertex(p0);

                GL.End();
                GL.PopMatrix();
            }
            else
            {
                if(PositionHandleArrowOnly)
                {
                    GL.End();
                }
                else
                {
                    Vector3 toCam = transform.inverse.MultiplyVector(camera.transform.position - position);

                    float fx = Mathf.Sign(Vector3.Dot(toCam, x)) * m_handleScale;
                    float fy = Mathf.Sign(Vector3.Dot(toCam, y)) * m_handleScale;
                    float fz = Mathf.Sign(Vector3.Dot(toCam, z)) * m_handleScale;

                    x.x *= fx;
                    y.y *= fy;
                    z.z *= fz;

                    Vector3 xy = x + y;
                    Vector3 xz = x + z;
                    Vector3 yz = y + z;

                    x = transform.MultiplyPoint(x);
                    y = transform.MultiplyPoint(y);
                    z = transform.MultiplyPoint(z);
                    xy = transform.MultiplyPoint(xy);
                    xz = transform.MultiplyPoint(xz);
                    yz = transform.MultiplyPoint(yz);

                    if (!xLocked && !zLocked)
                    {
                        GL.Color(selectedAxis != RuntimeHandleAxis.XZ ? m_colors.YColor : m_colors.SelectionColor);
                        GL.Vertex(position);
                        GL.Vertex(z);
                        GL.Vertex(z);
                        GL.Vertex(xz);
                        GL.Vertex(xz);
                        GL.Vertex(x);
                        GL.Vertex(x);
                        GL.Vertex(position);
                    }

                    if (!xLocked && !yLocked)
                    {
                        GL.Color(selectedAxis != RuntimeHandleAxis.XY ? m_colors.ZColor : m_colors.SelectionColor);
                        GL.Vertex(position);
                        GL.Vertex(y);
                        GL.Vertex(y);
                        GL.Vertex(xy);
                        GL.Vertex(xy);
                        GL.Vertex(x);
                        GL.Vertex(x);
                        GL.Vertex(position);
                    }

                    if (!yLocked && !zLocked)
                    {
                        GL.Color(selectedAxis != RuntimeHandleAxis.YZ ? m_colors.XColor : m_colors.SelectionColor);
                        GL.Vertex(position);
                        GL.Vertex(y);
                        GL.Vertex(y);
                        GL.Vertex(yz);
                        GL.Vertex(yz);
                        GL.Vertex(z);
                        GL.Vertex(z);
                        GL.Vertex(position);
                    }
                    GL.End();


                    GL.Begin(GL.QUADS);

                    if (!xLocked && !zLocked)
                    {
                        Color color = m_colors.YColor;
                        color.a = 0.5f;
                        GL.Color(color);
                        GL.Vertex(position);
                        GL.Vertex(z);
                        GL.Vertex(xz);
                        GL.Vertex(x);
                    }

                    if (!xLocked && !yLocked)
                    {
                        Color color = m_colors.ZColor;
                        color.a = 0.5f;
                        GL.Color(color);
                        GL.Vertex(position);
                        GL.Vertex(y);
                        GL.Vertex(xy);
                        GL.Vertex(x);
                    }

                    if (!yLocked && !zLocked)
                    {
                        Color color = m_colors.XColor;
                        color.a = 0.5f;
                        GL.Color(color);
                        GL.Vertex(position);
                        GL.Vertex(y);
                        GL.Vertex(yz);
                        GL.Vertex(z);
                    }

                    GL.End();
                }
            }
           
            m_shapesMaterial.SetPass(0);
            if(!xLocked && !yLocked && !zLocked)
            {
                Graphics.DrawMeshNow(Arrows, transform);
                if ((selectedAxis & RuntimeHandleAxis.X) != 0)
                {
                    Graphics.DrawMeshNow(SelectionArrowX, transform);
                }
                if ((selectedAxis & RuntimeHandleAxis.Y) != 0)
                {
                    Graphics.DrawMeshNow(SelectionArrowY, transform);
                }
                if ((selectedAxis & RuntimeHandleAxis.Z) != 0)
                {
                    Graphics.DrawMeshNow(SelectionArrowZ, transform);
                }
            }
            else
            {
                if (xLocked)
                {
                    Graphics.DrawMeshNow(DisabledArrowX, transform);
                }
                else
                {
                    if ((selectedAxis & RuntimeHandleAxis.X) != 0)
                    {
                        Graphics.DrawMeshNow(SelectionArrowX, transform);
                    }
                    else
                    {
                        Graphics.DrawMeshNow(ArrowX, transform);
                    }
                }

                if (yLocked)
                {
                    Graphics.DrawMeshNow(DisabledArrowY, transform);
                }
                else 
                {
                    if ((selectedAxis & RuntimeHandleAxis.Y) != 0)
                    {
                        Graphics.DrawMeshNow(SelectionArrowY, transform);
                    }
                    else
                    {
                        Graphics.DrawMeshNow(ArrowY, transform);
                    }     
                }

                if (zLocked)
                {
                    Graphics.DrawMeshNow(DisabledArrowZ, transform);
                }
                else 
                {
                    if ((selectedAxis & RuntimeHandleAxis.Z) != 0)
                    {
                        Graphics.DrawMeshNow(SelectionArrowZ, transform);
                    }
                    else
                    {
                        Graphics.DrawMeshNow(ArrowZ, transform);
                    }
                        
                }
            }
        }

        public void DoRotationHandle(Camera camera, Quaternion rotation, Vector3 position, RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None, LockObject lockObject = null, bool cameraFacingBillboardMode = true)
        {
            float screenScale = GetScreenScale(position, camera);
            float radius = m_handleScale;
            Vector3 scale = new Vector3(screenScale, screenScale, screenScale);
            Matrix4x4 xTranform = Matrix4x4.TRS(Vector3.zero, rotation * Quaternion.AngleAxis(-90, Vector3.up), Vector3.one);
            Matrix4x4 yTranform = Matrix4x4.TRS(Vector3.zero, rotation * Quaternion.AngleAxis(-90, Vector3.right), Vector3.one);
            Matrix4x4 zTranform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            Matrix4x4 objToWorld = Matrix4x4.TRS(position, Quaternion.identity, scale);

            bool xLocked = lockObject != null && lockObject.RotationX;
            bool yLocked = lockObject != null && lockObject.RotationY;
            bool zLocked = lockObject != null && lockObject.RotationZ;
            bool freeLocked = lockObject != null && lockObject.RotationFree;
            bool screenLocked = lockObject != null && lockObject.RotationScreen;

            if (cameraFacingBillboardMode)
            {
                m_linesMaterial.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(Matrix4x4.TRS(position, Quaternion.LookRotation(camera.transform.position - position), scale));
            }
            else
            {
                m_linesBillboardMaterial.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(objToWorld);
            }

            GL.Begin(GL.LINES);
            if (freeLocked)
                GL.Color(m_colors.DisabledColor);
            else
                GL.Color(selectedAxis != RuntimeHandleAxis.Free ? m_colors.AltColor : m_colors.SelectionColor);
            RuntimeGraphics.DrawCircleGL(Matrix4x4.identity, radius);

            if (screenLocked)
                GL.Color(m_colors.DisabledColor);
            else
                GL.Color(selectedAxis != RuntimeHandleAxis.Screen ? m_colors.AltColor : m_colors.SelectionColor);
            RuntimeGraphics.DrawCircleGL(Matrix4x4.identity, radius * 1.1f);
            GL.End();
            GL.PopMatrix();

            if(cameraFacingBillboardMode)
            {
                m_linesClipUsingClipPlaneMaterial.SetPass(0);
            }
            else
            {
                m_linesClipMaterial.SetPass(0);
            }
            
            GL.PushMatrix();
            GL.MultMatrix(objToWorld);
            GL.Begin(GL.LINES);
            if(xLocked)
                GL.Color(m_colors.DisabledColor);
            else
                GL.Color(selectedAxis != RuntimeHandleAxis.X ? m_colors.XColor : m_colors.SelectionColor);
            RuntimeGraphics.DrawCircleGL(xTranform, radius);

            if (yLocked)
                GL.Color(m_colors.DisabledColor);
            else
                GL.Color(selectedAxis != RuntimeHandleAxis.Y ? m_colors.YColor : m_colors.SelectionColor);
            RuntimeGraphics.DrawCircleGL(yTranform, radius);

            if (zLocked)
                GL.Color(m_colors.DisabledColor);
            else
                GL.Color(selectedAxis != RuntimeHandleAxis.Z ? m_colors.ZColor : m_colors.SelectionColor);
            RuntimeGraphics.DrawCircleGL(zTranform, radius);
            GL.End();

            GL.PopMatrix();

        }

        public void DoScaleHandle(Camera camera, Vector3 scale, Vector3 position, Quaternion rotation, RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None, LockObject lockObject = null)
        {
            float sScale = GetScreenScale(position, camera);
            Matrix4x4 linesTransform = Matrix4x4.TRS(position, rotation, scale * sScale);

            m_linesMaterial.SetPass(0);

            bool xLocked = lockObject != null && lockObject.ScaleX;
            bool yLocked = lockObject != null && lockObject.ScaleY;
            bool zLocked = lockObject != null && lockObject.ScaleZ;

            GL.Begin(GL.LINES);
            DoAxes(position, linesTransform, selectedAxis, xLocked, yLocked, zLocked);
            GL.End();
         
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, rotation, scale);
            m_shapesMaterial.SetPass(0);
            Vector3 screenScale = new Vector3(sScale, sScale, sScale);
            Vector3 xOffset = rotM.MultiplyVector(Vector3.right) * sScale * m_handleScale;
            Vector3 yOffset = rotM.MultiplyVector(Vector3.up) * sScale * m_handleScale;
            Vector3 zOffset = rotM.MultiplyPoint(Forward) * sScale * m_handleScale;
            if (selectedAxis == RuntimeHandleAxis.X)
            {  
                Graphics.DrawMeshNow(xLocked ? DisabledCube : SelectionCube, Matrix4x4.TRS(position + xOffset, rotation, screenScale));
                Graphics.DrawMeshNow(yLocked ? DisabledCube : CubeY, Matrix4x4.TRS(position + yOffset, rotation, screenScale));
                Graphics.DrawMeshNow(zLocked ? DisabledCube : CubeZ, Matrix4x4.TRS(position + zOffset, rotation, screenScale));
                Graphics.DrawMeshNow(xLocked && yLocked && zLocked ? DisabledCube : CubeUniform, Matrix4x4.TRS(position, rotation, screenScale * 1.35f));
            }
            else if (selectedAxis == RuntimeHandleAxis.Y)
            {
                Graphics.DrawMeshNow(xLocked ? DisabledCube : CubeX, Matrix4x4.TRS(position + xOffset, rotation, screenScale));
                Graphics.DrawMeshNow(yLocked ? DisabledCube : SelectionCube, Matrix4x4.TRS(position + yOffset, rotation, screenScale));
                Graphics.DrawMeshNow(zLocked ? DisabledCube : CubeZ, Matrix4x4.TRS(position + zOffset, rotation, screenScale));
                Graphics.DrawMeshNow(xLocked && yLocked && zLocked ? DisabledCube : CubeUniform, Matrix4x4.TRS(position, rotation, screenScale * 1.35f));
            }
            else if (selectedAxis == RuntimeHandleAxis.Z)
            {
                Graphics.DrawMeshNow(xLocked ? DisabledCube : CubeX, Matrix4x4.TRS(position + xOffset, rotation, screenScale));
                Graphics.DrawMeshNow(yLocked ? DisabledCube : CubeY, Matrix4x4.TRS(position + yOffset, rotation, screenScale));
                Graphics.DrawMeshNow(zLocked ? DisabledCube : SelectionCube, Matrix4x4.TRS(position + zOffset, rotation, screenScale));
                Graphics.DrawMeshNow(xLocked && yLocked && zLocked ? DisabledCube : CubeUniform, Matrix4x4.TRS(position, rotation, screenScale * 1.35f));
            }
            else if (selectedAxis == RuntimeHandleAxis.Free)
            {
                Graphics.DrawMeshNow(xLocked ? DisabledCube : CubeX, Matrix4x4.TRS(position + xOffset, rotation, screenScale));
                Graphics.DrawMeshNow(yLocked ? DisabledCube : CubeY, Matrix4x4.TRS(position + yOffset, rotation, screenScale));
                Graphics.DrawMeshNow(zLocked ? DisabledCube : CubeZ, Matrix4x4.TRS(position + zOffset, rotation, screenScale));
                Graphics.DrawMeshNow(xLocked && yLocked && zLocked ? DisabledCube : SelectionCube, Matrix4x4.TRS(position, rotation, screenScale * 1.35f));
            }
            else
            {
                Graphics.DrawMeshNow(xLocked ? DisabledCube : CubeX, Matrix4x4.TRS(position + xOffset, rotation, screenScale));
                Graphics.DrawMeshNow(yLocked ? DisabledCube : CubeY, Matrix4x4.TRS(position + yOffset, rotation, screenScale));
                Graphics.DrawMeshNow(zLocked ? DisabledCube : CubeZ, Matrix4x4.TRS(position + zOffset, rotation, screenScale));
                Graphics.DrawMeshNow(xLocked && yLocked && zLocked ? DisabledCube : CubeUniform, Matrix4x4.TRS(position, rotation, screenScale * 1.35f));
            }
        }

        public void DoSceneGizmo(Camera camera, Vector3 position, Quaternion rotation, Vector3 selection, float gizmoScale, Color textColor, float xAlpha = 1.0f, float yAlpha = 1.0f, float zAlpha = 1.0f)
        {
            float sScale = GetScreenScale(position, camera) * gizmoScale;
            Vector3 screenScale = new Vector3(sScale, sScale, sScale);

            const float billboardScale = 0.125f;
            float billboardOffset = 0.4f;
            if (camera.orthographic)
            {
                billboardOffset = 0.42f;
            }
            
            const float cubeScale = 0.15f;

            if (selection != Vector3.zero)
            {
                if (selection == Vector3.one)
                {
                    m_shapesMaterialZTestOffset.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoSelectedCube, Matrix4x4.TRS(position, rotation, screenScale * cubeScale));
                }
                else
                {
                    if ((xAlpha == 1.0f || xAlpha == 0.0f) && 
                        (yAlpha == 1.0f || yAlpha == 0.0f) && 
                        (zAlpha == 1.0f || zAlpha == 0.0f))
                    {
                        m_shapesMaterialZTestOffset.SetPass(0);
                        Graphics.DrawMeshNow(SceneGizmoSelectedAxis, Matrix4x4.TRS(position, rotation * Quaternion.LookRotation(selection, Vector3.up), screenScale));
                    }
                }
            }

            m_shapesMaterialZTest.color = Color.white;
            m_shapesMaterialZTest.SetPass(0);
            Graphics.DrawMeshNow(SceneGizmoCube, Matrix4x4.TRS(position, rotation, screenScale * cubeScale));
            if (xAlpha == 1.0f && yAlpha == 1.0f && zAlpha == 1.0f)
            {
                m_shapesMaterialZTest3.color = new Color(1, 1, 1, 1);
                m_shapesMaterialZTest3.SetPass(0);
                Graphics.DrawMeshNow(SceneGizmoXAxis, Matrix4x4.TRS(position, rotation, screenScale));
                m_shapesMaterialZTest4.color = new Color(1, 1, 1, 1);
                m_shapesMaterialZTest4.SetPass(0);
                Graphics.DrawMeshNow(SceneGizmoYAxis, Matrix4x4.TRS(position, rotation, screenScale));
                m_shapesMaterialZTest2.color = new Color(1, 1, 1, 1);
                m_shapesMaterialZTest2.SetPass(0);
                Graphics.DrawMeshNow(SceneGizmoZAxis, Matrix4x4.TRS(position, rotation, screenScale));

            }
            else
            {
                if (xAlpha < 1)
                {
                    m_shapesMaterialZTest3.color = new Color(1, 1, 1, yAlpha);
                    m_shapesMaterialZTest3.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoYAxis, Matrix4x4.TRS(position, rotation, screenScale));

                    m_shapesMaterialZTest4.color = new Color(1, 1, 1, zAlpha);
                    m_shapesMaterialZTest4.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoZAxis, Matrix4x4.TRS(position, rotation, screenScale));
                    
                    m_shapesMaterialZTest2.color = new Color(1, 1, 1, xAlpha);
                    m_shapesMaterialZTest2.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoXAxis, Matrix4x4.TRS(position, rotation, screenScale));
                   
                }
                else if (yAlpha < 1)
                {
                    m_shapesMaterialZTest4.color = new Color(1, 1, 1, zAlpha);
                    m_shapesMaterialZTest4.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoZAxis, Matrix4x4.TRS(position, rotation, screenScale));
                    
                    m_shapesMaterialZTest2.color = new Color(1, 1, 1, xAlpha);
                    m_shapesMaterialZTest2.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoXAxis, Matrix4x4.TRS(position, rotation, screenScale));

                    m_shapesMaterialZTest3.color = new Color(1, 1, 1, yAlpha);
                    m_shapesMaterialZTest3.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoYAxis, Matrix4x4.TRS(position, rotation, screenScale));
                }
                else
                {
                    m_shapesMaterialZTest2.color = new Color(1, 1, 1, xAlpha);
                    m_shapesMaterialZTest2.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoXAxis, Matrix4x4.TRS(position, rotation, screenScale));

                    m_shapesMaterialZTest3.color = new Color(1, 1, 1, yAlpha);
                    m_shapesMaterialZTest3.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoYAxis, Matrix4x4.TRS(position, rotation, screenScale));
                    
                    m_shapesMaterialZTest4.color = new Color(1, 1, 1, zAlpha);
                    m_shapesMaterialZTest4.SetPass(0);
                    Graphics.DrawMeshNow(SceneGizmoZAxis, Matrix4x4.TRS(position, rotation, screenScale));
                }
            }

            Color c = textColor;
            m_xMaterial.color = new Color(c.r, c.b, c.g, xAlpha);
            m_xMaterial.SetPass(0);
            DragSceneGizmoAxis(camera, position, rotation, Vector3.right, gizmoScale, billboardScale, billboardOffset, sScale);
            
            m_yMaterial.color = new Color(c.r, c.b, c.g, yAlpha);
            m_yMaterial.SetPass(0);
            DragSceneGizmoAxis(camera, position, rotation, Vector3.up, gizmoScale, billboardScale, billboardOffset, sScale);

            m_zMaterial.color = new Color(c.r, c.b, c.g, zAlpha);
            m_zMaterial.SetPass(0);
            DragSceneGizmoAxis(camera, position, rotation, Forward, gizmoScale, billboardScale, billboardOffset, sScale);

        }

        private void DragSceneGizmoAxis(Camera camera, Vector3 position, Quaternion rotation, Vector3 axis, float gizmoScale, float billboardScale, float billboardOffset, float sScale)
        {
            Vector3 reflectionOffset;

            reflectionOffset = Vector3.Reflect(camera.transform.forward, axis) * 0.1f;
            float dotAxis = Vector3.Dot(camera.transform.forward, axis);
            if (dotAxis > 0)
            {
                if(camera.orthographic)
                {
                    reflectionOffset += axis * dotAxis * 0.4f;
                }
                else
                {
                    reflectionOffset = axis * dotAxis * 0.7f;
                }
                
            }
            else
            {
                if (camera.orthographic)
                {
                    reflectionOffset -= axis * dotAxis * 0.1f;
                }
                else
                {
                    reflectionOffset = Vector3.zero;
                }
            }


            Vector3 pos = position + (axis + reflectionOffset) * billboardOffset * sScale;
            float scale = GetScreenScale(pos, camera) * gizmoScale;
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            Graphics.DrawMeshNow(SceneGizmoQuad, Matrix4x4.TRS(pos, rotation, scaleVector * billboardScale));
        }

        public static float GetGridFarPlane(Camera camera)
        {
            float h = camera.transform.position.y;
            float d = MathHelper.CountOfDigits(h);
            float spacing = Mathf.Pow(10, d - 1);
            return spacing * 150;
        }

        public void DrawGrid(Camera camera, Vector3 gridOffset, float camOffset = 0.0f)
        {
            float h = camOffset;
            h = Mathf.Abs(h);
            h = Mathf.Max(1, h);
            
            float d = MathHelper.CountOfDigits(h);
       
            float spacing = Mathf.Pow(10, d - 1);
            float nextSpacing = Mathf.Pow(10, d);
            float nextNextSpacing = Mathf.Pow(10, d + 1);

            float alpha = 1.0f - (h - spacing) / (nextSpacing - spacing);
            float alpha2 = (h * 10 - nextSpacing) / (nextNextSpacing - nextSpacing);

            Vector3 cameraPosition = camera.transform.position;
            DrawGrid(cameraPosition, gridOffset, spacing, alpha, h * 20);
            DrawGrid(cameraPosition, gridOffset, nextSpacing, alpha2, h * 20);
        }

        private void DrawGrid(Vector3 cameraPosition, Vector3 gridOffset, float spacing, float alpha ,float fadeDisance)
        {
            cameraPosition.y = gridOffset.y;

            gridOffset.y = 0;


            m_gridMaterial.SetFloat("_FadeDistance", fadeDisance);
            m_gridMaterial.SetPass(0);

            GL.Begin(GL.LINES);

            Color color = Colors.GridColor;
            color.a *= alpha;
            GL.Color(color);

            cameraPosition.x = Mathf.Floor(cameraPosition.x / spacing) * spacing;
            cameraPosition.z = Mathf.Floor(cameraPosition.z / spacing) * spacing;

            for (int i = -150; i < 150; ++i)
            {
                GL.Vertex(gridOffset + cameraPosition + new Vector3(i * spacing, 0, -150 * spacing));
                GL.Vertex(gridOffset + cameraPosition + new Vector3(i * spacing, 0, 150 * spacing));

                GL.Vertex(gridOffset + cameraPosition + new Vector3(-150 * spacing, 0, i * spacing));
                GL.Vertex(gridOffset + cameraPosition + new Vector3(150 * spacing, 0, i * spacing));
            }

            GL.End();
        }

        public Mesh CreateGridMesh(Color color, float spacing, int linesCount = 150)
        {
            int count = linesCount / 2;

            Mesh mesh = new Mesh();
            mesh.name = "Grid " + spacing;

            int index = 0;
            int[] indices = new int[count * 8];
            Vector3[] vertices = new Vector3[count * 8];
            Color[] colors = new Color[count * 8];
            
            for(int i = -count; i < count; ++i)
            {
                vertices[index] = new Vector3(i * spacing, 0, -count * spacing);
                vertices[index + 1] = new Vector3(i * spacing, 0, count * spacing);

                vertices[index + 2] = new Vector3(-count * spacing, 0, i * spacing);
                vertices[index + 3] = new Vector3(count * spacing, 0, i * spacing);

                indices[index] = index;
                indices[index + 1] = index + 1;
                indices[index + 2] = index + 2;
                indices[index + 3] = index + 3;

                colors[index] = colors[index + 1] = colors[index + 2] = colors[index + 3] = color;

                index += 4;
            }

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.colors = colors;

            return mesh;
        }

        public void DrawBounds(Camera camera, ref Bounds bounds, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            m_linesMaterialZTest.SetPass(0);

            Matrix4x4 transform = Matrix4x4.TRS(position, rotation, scale);

            GL.PushMatrix();
            GL.MultMatrix(transform);
            GL.Begin(GL.LINES);
            GL.Color(m_colors.BoundsColor);

            for(int i = -1; i <= 1; i += 2)
            {
                for(int j = -1; j <= 1; j += 2)
                {
                    for(int k = -1; k <= 1; k += 2)
                    {
                        Vector3 p = bounds.center + new Vector3(bounds.extents.x * i, bounds.extents.y * j, bounds.extents.z * k);
                        Vector3 pt = transform.MultiplyPoint(p);
                        float sScale = Mathf.Max(GetScreenScale(pt, camera), 0.1f);
                        Vector3 size = Vector3.one * 0.2f * sScale;

                        Vector3 sizeX = new Vector3(Mathf.Min(size.x / Mathf.Abs(scale.x), bounds.extents.x), 0, 0);
                        Vector3 sizeY = new Vector3(0, Mathf.Min(size.y / Mathf.Abs(scale.y), bounds.extents.y), 0);
                        Vector3 sizeZ = new Vector3(0, 0, Mathf.Min(size.z / Mathf.Abs(scale.z), bounds.extents.z));

                        DrawCorner(p, sizeX, sizeY, sizeZ, new Vector3(-1 * i, -1 * j, -1 * k));
                    }
                }
            }

            
            //DrawCorner(p1, sizeX, sizeY, sizeZ, new Vector3( 1,  1, -1));
            //DrawCorner(p2, sizeX, sizeY, sizeZ, new Vector3( 1, -1,  1));
            //DrawCorner(p3, sizeX, sizeY, sizeZ, new Vector3( 1, -1, -1));
            //DrawCorner(p4, sizeX, sizeY, sizeZ, new Vector3(-1,  1,  1));
            //DrawCorner(p5, sizeX, sizeY, sizeZ, new Vector3(-1,  1, -1));
            //DrawCorner(p6, sizeX, sizeY, sizeZ, new Vector3(-1, -1,  1));
            //DrawCorner(p7, sizeX, sizeY, sizeZ, new Vector3(-1, -1, -1));

            GL.End();
            GL.PopMatrix();
        }

        private static void DrawCorner(Vector3 p, Vector3 sizeX, Vector3 sizeY, Vector3 sizeZ, Vector3 s)
        {
            GL.Vertex(p);
            GL.Vertex(p + sizeX * s.x);
            GL.Vertex(p);
            GL.Vertex(p + sizeY * s.y);
            GL.Vertex(p);
            GL.Vertex(p + sizeZ * s.z);
            GL.Vertex(p);
            GL.Vertex(p + sizeX * s.x);
            GL.Vertex(p);
            GL.Vertex(p + sizeY * s.y);
            GL.Vertex(p);
            GL.Vertex(p + sizeZ * s.z);
        }

     
    }

}
