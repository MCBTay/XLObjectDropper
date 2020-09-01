using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Battlehub.RTCommon
{
    public class RTEVRGraphicsRaycaster : OVRRaycaster
    {
        public RuntimeWindow SceneWindow;

        protected override void Awake()
        {
            base.Awake();
            blockingObjects = BlockingObjects.All;
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            base.Raycast(eventData, resultAppendList);

            if (resultAppendList.Count == 0 && SceneWindow != null)
            {
                var castResult = new RaycastResult
                {
                    gameObject = SceneWindow.gameObject,
                    module = this,
                    distance = 0,
                    index = resultAppendList.Count,
                    depth = 0,
                    worldPosition = SceneWindow.transform.position,
                };

                resultAppendList.Add(castResult);
            }
        }
    }

}
