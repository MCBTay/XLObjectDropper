using Battlehub.RTCommon;
using Battlehub.Utils;
using UnityEngine;

using UnityObject = UnityEngine.Object;
namespace Battlehub.RTCommon
{
    public interface IResourcePreviewUtility
    {
        byte[] CreatePreviewData(UnityObject obj);
        Texture2D TakeSnapshot(GameObject go);
    }

    public class ResourcePreviewUtility : MonoBehaviour, IResourcePreviewUtility
    {
        [SerializeField]
        private ObjectToTexture m_objectToTextureCamera = null;
  
        [SerializeField]
        private GameObject m_fallbackPrefab = null;

        [SerializeField]
        private Vector3 m_scale = new Vector3(0.9f, 0.9f, 0.9f);

        private Shader m_unlitTexShader;

        private void Awake()
        {
            IRTE rte = IOC.Resolve<IRTE>();

            m_unlitTexShader = Shader.Find("Unlit/Texture");
            if(m_objectToTextureCamera == null)
            {
                GameObject objectToTextureGO = new GameObject("Object To Texture");
                objectToTextureGO.SetActive(false);
                objectToTextureGO.transform.SetParent(transform, false);

                Camera camera = objectToTextureGO.AddComponent<Camera>();
                camera.nearClipPlane = 0.01f;
                camera.orthographic = true;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.stereoTargetEye = StereoTargetEyeMask.None;
                camera.cullingMask = 1 << rte.CameraLayerSettings.ResourcePreviewLayer;

                m_objectToTextureCamera = objectToTextureGO.AddComponent<ObjectToTexture>();
                m_objectToTextureCamera.objectImageLayer = rte.CameraLayerSettings.ResourcePreviewLayer;


                Light[] lights = FindObjectsOfType<Light>();
                for(int i = 0; i < lights.Length; ++i)
                {
                    lights[i].cullingMask &= ~(1 << rte.CameraLayerSettings.ResourcePreviewLayer);
                }

                GameObject lightGO = new GameObject("Directional light");
                lightGO.transform.SetParent(objectToTextureGO.transform, false);
                lightGO.layer = rte.CameraLayerSettings.ResourcePreviewLayer;
                lightGO.transform.rotation = Quaternion.Euler(30, 0, 0);

                Light light = lightGO.AddComponent<Light>();
                light.type = LightType.Directional;
                light.cullingMask = 1 << rte.CameraLayerSettings.ResourcePreviewLayer;
            }

        }

        private void OnDestroy()
        {
         
        }

        private byte[] TakeSnapshotBytes(GameObject obj)
        {
            Texture2D texture = TakeSnapshot(obj);

            byte[] result;
            if (texture != null)
            {
                result = texture.EncodeToPNG();
                Destroy(texture);
            }
            else
            {
                result = new byte[0];
            }
            
            return result;
        }

        public Texture2D TakeSnapshot(GameObject obj)
        {
            m_objectToTextureCamera.defaultScale = m_scale;
            m_objectToTextureCamera.gameObject.SetActive(true);
            Texture2D texture = m_objectToTextureCamera.TakeObjectSnapshot(obj, m_fallbackPrefab);
            m_objectToTextureCamera.gameObject.SetActive(false);
            return texture;
        }

        public byte[] CreatePreviewData(UnityObject obj)
        {
            byte[] previewData = new byte[0];
            if (obj is GameObject)
            {
                GameObject go = (GameObject)obj;
                previewData = TakeSnapshotBytes(go);
            }
            else if (obj is Material)
            {
                Material material = (Material)obj;
                Shader shader = material.shader;
                bool replaceParticlesShader = shader != null && shader.name.StartsWith("Particles/");
                if (replaceParticlesShader)
                {
                    material.shader = m_unlitTexShader;
                }

                GameObject materialSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                materialSphere.transform.position = Vector3.zero;

                MeshRenderer renderer = materialSphere.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = material;

                previewData = TakeSnapshotBytes(materialSphere);
                DestroyImmediate(materialSphere);

                if (replaceParticlesShader)
                {
                    material.shader = shader;
                }
            }
            else if (obj is Texture2D)
            {
                Texture2D texture = (Texture2D)obj;
                bool isReadable = texture.isReadable;
                bool isSupportedFormat = texture.format == TextureFormat.ARGB32 ||
                                      texture.format == TextureFormat.RGBA32 ||
                                      texture.format == TextureFormat.RGB24 ||
                                      texture.format == TextureFormat.Alpha8;

                if(isReadable && isSupportedFormat)
                {
                    texture = Instantiate(texture);
                }
                else
                {
                    texture = texture.DeCompress();
                }

                float textureAspect = (texture.width * m_objectToTextureCamera.snapshotTextureHeight) / (float)Mathf.Max(1, texture.height * m_objectToTextureCamera.snapshotTextureWidth);
                TextureScale.Bilinear(texture, Mathf.RoundToInt(m_objectToTextureCamera.snapshotTextureWidth * textureAspect), m_objectToTextureCamera.snapshotTextureHeight);
                previewData = texture.EncodeToPNG();
                Destroy(texture);
            }
            else if (obj is Sprite)
            {
                Sprite sprite = (Sprite)obj;
                previewData = FromSprite(sprite);
            }

            return previewData;
        }

        private byte[] FromSprite(Sprite sprite)
        {
            byte[] previewData = null;
            if (sprite.texture != null && sprite.texture.isReadable)
            {
                Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                texture.SetPixels(newColors);
                texture.Resize(m_objectToTextureCamera.snapshotTextureWidth, m_objectToTextureCamera.snapshotTextureHeight);
                previewData = texture.EncodeToPNG();

                Destroy(texture);
            }

            return previewData;
        }
    }
}



