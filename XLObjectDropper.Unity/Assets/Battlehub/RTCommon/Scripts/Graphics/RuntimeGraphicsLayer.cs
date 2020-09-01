using UnityEngine;

namespace Battlehub.RTCommon
{
    [DefaultExecutionOrder(-55)]
    [RequireComponent(typeof(RuntimeWindow))]
    public class RuntimeGraphicsLayer : MonoBehaviour
    {
        [SerializeField]
        private Camera m_graphicsLayerCamera;

        private RenderTextureCamera m_renderTextureCamera;
        
        private RuntimeWindow m_editorWindow;

        public RuntimeWindow Window
        {
            get { return m_editorWindow; }
        }
        
        private void Awake()
        {
            m_editorWindow = GetComponent<RuntimeWindow>();
            PrepareGraphicsLayerCamera();
        }

        private void Start()
        {
            if (m_editorWindow.Index >= m_editorWindow.Editor.CameraLayerSettings.MaxGraphicsLayers)
            {
                Debug.LogError("m_editorWindow.Index >= m_editorWindow.Editor.CameraLayerSettings.MaxGraphicsLayers");
            }
        }

        private void OnDestroy()
        {
            if(m_graphicsLayerCamera != null)
            {
                Destroy(m_graphicsLayerCamera.gameObject);
            }

            if(m_renderTextureCamera != null && m_renderTextureCamera.OverlayMaterial != null)
            {
                Destroy(m_renderTextureCamera.OverlayMaterial);
            }
        }

        private void PrepareGraphicsLayerCamera()
        {
            bool wasActive = m_editorWindow.Camera.gameObject.activeSelf;
            m_editorWindow.Camera.gameObject.SetActive(false);

            //m_trackedPoseDriver = m_editorWindow.Camera.GetComponent<TrackedPoseDriver>();
            if (m_editorWindow.Editor.IsVR && m_editorWindow.Camera.stereoEnabled && m_editorWindow.Camera.stereoTargetEye == StereoTargetEyeMask.Both )
            {
                m_graphicsLayerCamera = Instantiate(m_editorWindow.Camera, m_editorWindow.Camera.transform.parent);
                m_graphicsLayerCamera.transform.SetSiblingIndex(m_editorWindow.Camera.transform.GetSiblingIndex() + 1);
            }
            else
            {
                m_graphicsLayerCamera = Instantiate(m_editorWindow.Camera, m_editorWindow.Camera.transform);
            }

            for (int i = m_graphicsLayerCamera.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(m_graphicsLayerCamera.transform.GetChild(i).gameObject);
            }

            Component[] components = m_graphicsLayerCamera.GetComponents<Component>();
            for (int i = 0; i < components.Length; ++i)
            {
                Component component = components[i];
                if (component is Transform)
                {
                    continue;
                }
                if (component is Camera)
                {
                    continue;
                }
                if(component is RenderTextureCamera)
                {
                    continue;
                }

                Destroy(component);
            }

            m_graphicsLayerCamera.transform.localPosition = Vector3.zero;
            m_graphicsLayerCamera.transform.localRotation = Quaternion.identity;
            m_graphicsLayerCamera.transform.localScale = Vector3.one;
            m_graphicsLayerCamera.name = "GraphicsLayerCamera";
            m_graphicsLayerCamera.depth = m_editorWindow.Camera.depth + 1;
            m_graphicsLayerCamera.cullingMask = 1 << (m_editorWindow.Editor.CameraLayerSettings.RuntimeGraphicsLayer + m_editorWindow.Index);

            m_renderTextureCamera = m_graphicsLayerCamera.GetComponent<RenderTextureCamera>();
            if (m_renderTextureCamera == null)
            {
                #if UNITY_2019_1_OR_NEWER
                if(RenderPipelineInfo.Type != RPType.Standard)
                {
                    UnityEngine.Rendering.RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
                }
                #endif
                m_graphicsLayerCamera.clearFlags = CameraClearFlags.Depth;
            }
            else
            {
                m_renderTextureCamera.OverlayMaterial = new Material(Shader.Find("Battlehub/RTCommon/RenderTextureOverlay"));
                m_graphicsLayerCamera.clearFlags = CameraClearFlags.SolidColor;
                m_graphicsLayerCamera.backgroundColor = new Color(0, 0, 0, 0);
            }

            m_graphicsLayerCamera.allowHDR = false; //fix strange screen blinking bug...
            m_graphicsLayerCamera.projectionMatrix = m_editorWindow.Camera.projectionMatrix; //for ARCore
            
            
            m_editorWindow.Camera.gameObject.SetActive(wasActive);
            m_graphicsLayerCamera.gameObject.SetActive(wasActive);
        }

        #if UNITY_2019_1_OR_NEWER
        private void OnEndFrameRendering(UnityEngine.Rendering.ScriptableRenderContext arg1, Camera[] arg2)
        {
            UnityEngine.Rendering.RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;

            //LWRP OR HDRP;

            bool wasActive = m_graphicsLayerCamera.gameObject.activeSelf;
            m_graphicsLayerCamera.gameObject.SetActive(false);

            m_renderTextureCamera = m_graphicsLayerCamera.gameObject.AddComponent<RenderTextureCamera>();
            //m_renderTextureCamera.Fullscreen = false;

            IRTE rte = IOC.Resolve<IRTE>();
            RuntimeWindow sceneWindow = rte.GetWindow(RuntimeWindowType.Scene);
            m_renderTextureCamera.OutputRoot = (RectTransform)sceneWindow.transform;
            m_renderTextureCamera.OverlayMaterial = new Material(Shader.Find("Battlehub/RTCommon/RenderTextureOverlay"));
            m_graphicsLayerCamera.clearFlags = CameraClearFlags.SolidColor;
            m_graphicsLayerCamera.backgroundColor = new Color(0, 0, 0, 0);

            m_graphicsLayerCamera.gameObject.SetActive(wasActive);
        }
        #endif

        private void LateUpdate()
        {
            if(m_graphicsLayerCamera.depth != m_editorWindow.Camera.depth + 1)
            {
                m_graphicsLayerCamera.depth = m_editorWindow.Camera.depth + 1;
            }

            if (m_graphicsLayerCamera.fieldOfView != m_editorWindow.Camera.fieldOfView)
            {
                m_graphicsLayerCamera.fieldOfView = m_editorWindow.Camera.fieldOfView;
            }

            if (m_graphicsLayerCamera.orthographic != m_editorWindow.Camera.orthographic)
            {
                m_graphicsLayerCamera.orthographic = m_editorWindow.Camera.orthographic;
            }

            if (m_graphicsLayerCamera.orthographicSize != m_editorWindow.Camera.orthographicSize)
            {
                m_graphicsLayerCamera.orthographicSize = m_editorWindow.Camera.orthographicSize;
            }

            if (m_graphicsLayerCamera.rect != m_editorWindow.Camera.rect)
            {
                m_graphicsLayerCamera.rect = m_editorWindow.Camera.rect;
            }

            if(m_graphicsLayerCamera.enabled != m_editorWindow.Camera.enabled)
            {
                m_graphicsLayerCamera.enabled = m_editorWindow.Camera.enabled;
            }

            //if(m_graphicsLayerCamera.gameObject.activeSelf != m_editorWindow.Camera.gameObject.activeSelf)
            //{
            //    m_graphicsLayerCamera.gameObject.SetActive(m_editorWindow.Camera.gameObject.activeSelf);
            //}
            
            //if(m_trackedPoseDriver != null)

            if (m_editorWindow.Camera.pixelWidth > 0 && m_editorWindow.Camera.pixelHeight > 0)
            {
                m_graphicsLayerCamera.projectionMatrix = m_editorWindow.Camera.projectionMatrix; //ARCore
            }

        }
    }
}



