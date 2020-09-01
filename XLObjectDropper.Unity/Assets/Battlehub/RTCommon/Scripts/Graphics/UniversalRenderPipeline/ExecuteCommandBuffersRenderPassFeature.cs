using UnityEngine;
using UnityEngine.Rendering;
#if FALSE && UNITY_POST_PROCESSING_STACK_V2 //lwrp define constant exists?
using UnityEngine.Rendering.LWRP;

namespace Battlehub.RTCommon
{
    public class ExecuteCommandBuffersRenderPassFeature : ScriptableRendererFeature
    {
        class CustomRenderPass : ScriptableRenderPass
        {
            // This method is called before executing the render pass. 
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in an performance manner.
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
               
            }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.camera.commandBufferCount > 0)
                {
                    CommandBuffer[] cmdBuffer = renderingData.cameraData.camera.GetCommandBuffers(CameraEvent.BeforeImageEffects);
                    for(int i = 0; i < cmdBuffer.Length; ++i)
                    {
                        context.ExecuteCommandBuffer(cmdBuffer[i]);
                    }
                }
            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd)
            {
            }
        }

        CustomRenderPass m_ScriptablePass;

        public override void Create()
        {
            Debug.Log("Create ExecuteCommandBuffersRenderPassFeature");

            m_ScriptablePass = new CustomRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
#endif

