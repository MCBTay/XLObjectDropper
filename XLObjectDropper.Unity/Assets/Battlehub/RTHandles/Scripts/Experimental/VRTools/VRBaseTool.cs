using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class VRBaseTool : RTEComponent
    {
        public RuntimeSceneVRComponent Component
        {
            get;
            set;
        }

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            RuntimeGraphicsLayer graphicsLayer = Window.GetComponent<RuntimeGraphicsLayer>();
            if (graphicsLayer == null)
            {
                graphicsLayer = Window.gameObject.AddComponent<RuntimeGraphicsLayer>();
            }

            SetLayer(transform, Window.Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index);

        }

        private void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                SetLayer(child, layer);
            }
        }
    }
}
