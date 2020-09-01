using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
namespace Battlehub.RTHandles
{
    public static class BoxSelectionRenderer 
    {
        private static RenderTextureFormat s_renderTextureFormat;
        private static bool s_initialized;
        private static RenderTextureFormat[] s_preferredFormats = new RenderTextureFormat[]
        {
            RenderTextureFormat.ARGB32,
            RenderTextureFormat.ARGBFloat,
        };

        private static RenderTextureFormat RenderTextureFormat
        {
            get
            {
                Init();
                return s_renderTextureFormat;
            }
        }

        private static TextureFormat TextureFormat { get { return TextureFormat.ARGB32; } }

        private static Shader s_objectSelectionShader;
        private static Shader ObjectSelectionShader
        {
            get
            {
                Init();
                return s_objectSelectionShader;
            }
        }

        private static void Init()
        {
            if(s_initialized)
            {
                return;
            }

            s_initialized = true;
            s_objectSelectionShader = Shader.Find("Battlehub/RTHandles/BoxSelectionShader");
                        
            for (int i = 0; i < s_preferredFormats.Length; i++)
            {
                if (SystemInfo.SupportsRenderTextureFormat(s_preferredFormats[i]))
                {
                    s_renderTextureFormat = s_preferredFormats[i];
                    break;
                }
            }
        }

        public static Renderer[] PickObjectsInRect(
            Camera camera,
            Rect selectionRect,
            Renderer[] renderers,
            int renderTextureWidth = -1,
            int renderTextureHeight = -1)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                Material[] materials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
                {
                    //disable instancing when done? do I need to enable it at all
                    materials[materialIndex].enableInstancing = true;

                    //is this really needed to create new instance each time?
                    MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(propertyBlock, materialIndex);
                    propertyBlock.SetColor("_SelectionColor", EncodeRGBA((uint)i + 1));
                    renderer.SetPropertyBlock(propertyBlock, materialIndex);
                }
            }

            Texture2D tex = RenderWithReplacementShader(camera, ObjectSelectionShader, null, renderTextureWidth, renderTextureHeight);

            Color32[] pix = tex.GetPixels32();
            int ox = System.Math.Max(0, Mathf.FloorToInt(selectionRect.x));
            int oy = System.Math.Max(0, Mathf.FloorToInt((tex.height - selectionRect.y) - selectionRect.height));
            int imageWidth = tex.width;
            int imageHeight = tex.height;
            int width = Mathf.FloorToInt(selectionRect.width);
            int height = Mathf.FloorToInt(selectionRect.height);
            UnityObject.DestroyImmediate(tex);

            List<Renderer> selectedRenderers = new List<Renderer>();
            HashSet<int> used = new HashSet<int>();

            for (int y = oy; y < System.Math.Min(oy + height, imageHeight); y++)
            {
                for (int x = ox; x < System.Math.Min(ox + width, imageWidth); x++)
                {
                    int index = (int)DecodeRGBA(pix[y * imageWidth + x]) - 1;
                    if(index < 0 || index >= renderers.Length)
                    {
                        continue;
                    }

                    if (used.Add(index))
                    {
                        Renderer selectedRenderer = renderers[index];
                        selectedRenderers.Add(selectedRenderer);
                    }
                }
            }

            return selectedRenderers.ToArray();
        }

        private static uint DecodeRGBA(Color32 color)
        {
            uint r = (uint)color.r;
            uint g = (uint)color.g;
            uint b = (uint)color.b;

            if (System.BitConverter.IsLittleEndian)
                return r << 16 | g << 8 | b;
            else
                return r << 24 | g << 16 | b << 8;
        }

        private static Color32 EncodeRGBA(uint hash)
        {
            // skip using BitConverter.GetBytes since this is super simple
            // bit math, and allocating arrays for each conversion is expensive
            if (System.BitConverter.IsLittleEndian)
                return new Color32(
                    (byte)(hash >> 16 & 0xFF),
                    (byte)(hash >> 8 & 0xFF),
                    (byte)(hash & 0xFF),
                    (byte)(255));
            else
                return new Color32(
                    (byte)(hash >> 24 & 0xFF),
                    (byte)(hash >> 16 & 0xFF),
                    (byte)(hash >> 8 & 0xFF),
                    (byte)(255));
        }

        private static Texture2D RenderWithReplacementShader(
            Camera camera,
            Shader shader,
            string tag,
            int width = -1,
            int height = -1)
        {
            bool autoSize = width < 0 || height < 0;

            int _width = autoSize ? (int)camera.pixelRect.width : width;
            int _height = autoSize ? (int)camera.pixelRect.height : height;

            GameObject go = new GameObject();
            Camera renderCam = go.AddComponent<Camera>();
            renderCam.CopyFrom(camera);

            renderCam.renderingPath = RenderingPath.Forward;
            renderCam.enabled = false;
            renderCam.clearFlags = CameraClearFlags.SolidColor;
            renderCam.backgroundColor = Color.white;

            renderCam.allowHDR = false;
            renderCam.allowMSAA = false;
            renderCam.forceIntoRenderTexture = true;

            RenderTextureDescriptor descriptor = new RenderTextureDescriptor()
            {
                width = _width,
                height = _height,
                colorFormat = RenderTextureFormat,
                autoGenerateMips = false,
                depthBufferBits = 16,
                dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
                enableRandomWrite = false,
                memoryless = RenderTextureMemoryless.None,
                sRGB = false,
                useMipMap = false,
                volumeDepth = 1,
                msaaSamples = 1
            };
            RenderTexture rt = RenderTexture.GetTemporary(descriptor);


            RenderTexture prev = RenderTexture.active;
            renderCam.targetTexture = rt;
            RenderTexture.active = rt;

            Debug.Log(shader.name);
            renderCam.RenderWithShader(shader, tag);

            Texture2D img = new Texture2D(_width, _height, TextureFormat, false, false);
            img.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            img.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            UnityObject.DestroyImmediate(go);

            return img;
        }
    }
}

