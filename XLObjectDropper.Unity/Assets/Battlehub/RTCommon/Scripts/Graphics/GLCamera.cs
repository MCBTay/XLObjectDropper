using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
 
    /// <summary>
    /// Camera behavior for GL. rendering
    /// </summary>
    [ExecuteInEditMode]
    public class GLCamera : MonoBehaviour
    {
        public int CullingMask = -1;

        private Camera m_camera;

        private void Awake()
        {
            m_camera = GetComponent<Camera>();
#if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
#endif
        }

        private void OnDestroy()
        {
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
                GL.LoadProjectionMatrix(GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));

                if (m_camera == camera)
                {
                    if (GLRenderer.Instance != null)
                    {
                        GLRenderer.Instance.Draw(CullingMask, camera);
                    }
                }
            }
            finally { GL.PopMatrix(); }  
        }
#endif

        private void OnPostRender()
        { 
            if(GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Draw(CullingMask, Camera.current);
            }
        }
    }
}

