using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTCommon
{
    public enum RPType
    {
        Unknown,
        Standard,
        LWRP,
        HDRP,
        URP,
    }

    public static class RenderPipelineInfo 
    {
        public static bool ForceUseRenderTextures = true;
        public static bool UseRenderTextures
        {
            get { return Type != RPType.Standard || ForceUseRenderTextures; }
        }

        public static readonly RPType Type;
        public static readonly string DefaultShaderName;
        public static readonly int MSAASampleCount;

        private static Material m_defaultMaterial;
        public static Material DefaultMaterial
        {
            get
            {
                if(m_defaultMaterial == null)
                {
                    m_defaultMaterial = new Material(Shader.Find(DefaultShaderName));
                }

                return m_defaultMaterial;
            }
        }

        static RenderPipelineInfo()
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                Type = RPType.Standard;
                DefaultShaderName = "Standard";
                MSAASampleCount = QualitySettings.antiAliasing;
            }
            else
            {
                Type pipelineType = GraphicsSettings.renderPipelineAsset.GetType();
                if (pipelineType.Name == "LightweightRenderPipelineAsset")
                {
                    Type = RPType.LWRP;
                    MSAASampleCount = GetMSAASampleCount(pipelineType);
                    DefaultShaderName = "Lightweight Render Pipeline/Lit";
                }
                else if (pipelineType.Name == "UniversalRenderPipelineAsset")
                {
                    Type = RPType.URP;
                    MSAASampleCount = GetMSAASampleCount(pipelineType);
                    DefaultShaderName = "Universal Render Pipeline/Lit";
                }
                else if (pipelineType.Name == "HDRenderRenderPipelineAsset")
                {
                    Type = RPType.HDRP;
                    MSAASampleCount = GetMSAASampleCount(pipelineType);
                    DefaultShaderName = "HD Render Pipeline/Lit";
                }
                else
                {
                    Debug.Log(GraphicsSettings.renderPipelineAsset.GetType());
                    Type = RPType.Unknown;
                    MSAASampleCount = 0;
                }
            }
           
        }

        static int GetMSAASampleCount(Type pipelineType)
        {
            PropertyInfo msaaProp = pipelineType.GetProperty("msaaSampleCount");
            if (msaaProp != null)
            {
                return Convert.ToInt32(msaaProp.GetValue(GraphicsSettings.renderPipelineAsset));
            }
            return 0;
        }


    }
}

