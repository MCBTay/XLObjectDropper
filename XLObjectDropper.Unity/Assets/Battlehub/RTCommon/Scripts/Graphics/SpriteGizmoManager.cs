using Battlehub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Battlehub.RTCommon
{
    public interface ISpriteGizmoManager
    {
        void Register(Type type, Material material);
        void Refresh();
    }

    [DefaultExecutionOrder(-1)]
    public class SpriteGizmoManager : MonoBehaviour, ISpriteGizmoManager
    {
        private readonly Dictionary<Type, string> m_builtIn = new Dictionary<Type, string>
            {
                {  typeof(Light), "BattlehubLightGizmo" },
                {  typeof(Camera), "BattlehubCameraGizmo" },
                {  typeof(AudioSource), "BattlehubAudioSourceGizmo" }
            };

        private Dictionary<Type, Material> m_registered = new Dictionary<Type, Material>();
        private Dictionary<Type, Material> m_typeToMaterial;
        private Type[] m_types;
        private IRTE m_editor;

        [SerializeField]
        private float m_gizmoScale = 1;
        public float GizmoScale
        {
            get { return m_gizmoScale; }
            set { m_gizmoScale = value; }
        }

        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            if(m_editor == null)
            {
                Debug.LogError("RTE is null");
            }

            IOC.RegisterFallback<ISpriteGizmoManager>(this);
            AwakeOverride();
        }

        private void Start()
        {
            Refresh();
            StartOverride();
        }

        private void OnDestroy()
        {
            Cleanup();

            m_typeToMaterial = null;
            m_types = null;

            OnDestroyOverride();

            IOC.UnregisterFallback<ISpriteGizmoManager>(this);
        }

        protected virtual void AwakeOverride()
        {

        }

        protected virtual void StartOverride()
        {

        }

        protected virtual void OnDestroyOverride()
        {

        }

        protected virtual Type[] GetTypes(Type[] types)
        {
            return types;
        }

        public void Register(Type type, Material material)
        {
            m_registered[type] = material;
        }

        public void Refresh()
        {
            Cleanup();
            Initialize();
        }

        protected virtual void GreateGizmo(GameObject go, Type type)
        {
            Material material;
            if (m_typeToMaterial.TryGetValue(type, out material))
            {
                SpriteGizmo gizmo = go.GetComponent<SpriteGizmo>();
                if (!gizmo)
                {
                    gizmo = go.AddComponent<SpriteGizmo>();
                }

                gizmo.Material = material;
            }
        }

        protected virtual void DestroyGizmo(GameObject go)
        {
            SpriteGizmo gizmo = go.GetComponent<SpriteGizmo>();
            if (gizmo)
            {
                Destroy(gizmo);
            }
        }

        private void Initialize()
        {
            if (m_types != null)
            {
                Debug.LogWarning("Already initialized");
                return;
            }

            m_typeToMaterial = new Dictionary<Type, Material>();
            foreach(KeyValuePair<Type, Material> kvp in m_registered)
            {
                if (kvp.Value != null)
                {
                    m_typeToMaterial.Add(kvp.Key, kvp.Value);
                }   
            }

            foreach (KeyValuePair<Type, string> kvp in m_builtIn)
            {
                if(m_typeToMaterial.ContainsKey(kvp.Key))
                {
                    continue;
                }

                Material material = Resources.Load<Material>(kvp.Value);
                if (material != null)
                {
                    m_typeToMaterial.Add(kvp.Key, material);
                }
            }

            int index = 0;
            m_types = new Type[m_typeToMaterial.Count];
            foreach (Type type in m_typeToMaterial.Keys)
            {
                m_types[index] = type;
                index++;
            }

            m_types = GetTypes(m_types);
            OnIsOpenedChanged();
            m_editor.IsOpenedChanged += OnIsOpenedChanged;
        }

        private void Cleanup()
        {
            m_types = null;
            m_typeToMaterial = null;
            if(m_editor != null)
            {
                m_editor.IsOpenedChanged -= OnIsOpenedChanged;
            }
            UnsubscribeAndDestroy();
        }

        private void UnsubscribeAndDestroy()
        {
            Unsubscribe();

            SpriteGizmo[] objs = Resources.FindObjectsOfTypeAll<SpriteGizmo>();
            for (int j = 0; j < objs.Length; ++j)
            {
                SpriteGizmo obj = objs[j];
                if (!obj.gameObject.IsPrefab())
                {
                    DestroyGizmo(obj.gameObject);
                }
            }
        }

        private void OnIsOpenedChanged()
        {
            if (m_editor.IsOpened)
            {
                IEnumerable<ExposeToEditor> objects = m_editor.Object.Get(false);

                for (int i = 0; i < m_types.Length; ++i)
                {
                    IEnumerable<ExposeToEditor> objectsOfType = objects.Where(o => o.GetComponent(m_types[i]) != null);
                    foreach (ExposeToEditor obj in objectsOfType)
                    {
                        GreateGizmo(obj.gameObject, m_types[i]);
                    }
                }

                Subscribe();
            }
            else
            {
                UnsubscribeAndDestroy();
            }
        }

        private void Subscribe()
        {
            m_editor.Object.Awaked += OnAwaked;
            m_editor.Object.Destroyed += OnDestroyed;
        }

        private void Unsubscribe()
        {
            if(m_editor != null && m_editor.Object != null)
            {
                m_editor.Object.Awaked -= OnAwaked;
                m_editor.Object.Destroyed -= OnDestroyed;
            }
        }

        private void OnAwaked(ExposeToEditor obj)
        {
            for (int i = 0; i < m_types.Length; ++i)
            {
                Component component = obj.GetComponent(m_types[i]);
                if (component != null)
                {
                    GreateGizmo(obj.gameObject, m_types[i]);
                }
            }
        }

        private void OnDestroyed(ExposeToEditor obj)
        {
            for (int i = 0; i < m_types.Length; ++i)
            {
                Component component = obj.GetComponent(m_types[i]);
                if (component != null)
                {
                    DestroyGizmo(obj.gameObject);
                }
            }
        }
    }
}
