using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XLObjectDropper.UI
{
    public class ObjectPlacement_Manager : MonoBehaviour
    {
        [Header("Object Placement Elements")]
        // Object Placement
        public GameObject MainScreen_UI;
        [Space(10)]
        public GameObject RB_UI;
        public GameObject LB_UI;
        [Space(10)]
        public GameObject RT_UI;
        public GameObject LT_UI;
        [Space(10)]
        public GameObject Dpad_Up;
        public GameObject Dpad_Down;
        public GameObject Dpad_Left;
        public GameObject Dpad_Right;

        void start()
        {
            // Object Placement
            RB_UI.SetActive(false);
            LB_UI.SetActive(false);
            RT_UI.SetActive(false);
            LT_UI.SetActive(false);
            Dpad_Down.SetActive(false);
            Dpad_Up.SetActive(false);
            Dpad_Left.SetActive(false);
            Dpad_Right.SetActive(false);
        }
    }
}
