
using UnityEngine;

namespace Battlehub.RTHandles
{
    public delegate void VRRecenterEvent(Quaternion quaternion);

    public class VRRecenterTool : VRBaseTool
    {
        [SerializeField]
        private VRRecenterCtrl m_recenterCtrlPrefab = null;

        [SerializeField]
        private float m_uiDistance = 10;

        public event VRRecenterEvent Recenter;

        protected virtual void Start()
        {
            VRRecenterCtrl plusZ = InitCtrl();
            VRRecenterCtrl minusZ = InitCtrl();
            VRRecenterCtrl plusY = InitCtrl();
            VRRecenterCtrl minusY = InitCtrl();
            VRRecenterCtrl plusX = InitCtrl();
            VRRecenterCtrl minusX = InitCtrl();

            plusZ.transform.position = Vector3.forward * m_uiDistance;
            plusZ.Text = "+Z";

            minusZ.transform.position = Vector3.back * m_uiDistance;
            minusZ.transform.rotation = Quaternion.LookRotation(Vector3.back);
            minusZ.Text = "-Z";

            plusY.transform.position = Vector3.up * m_uiDistance;
            plusY.transform.rotation = Quaternion.LookRotation(Vector3.up);
            plusY.Text = "+Y";

            minusY.transform.position = Vector3.down * m_uiDistance;
            minusY.transform.rotation = Quaternion.LookRotation(Vector3.down);
            minusY.Text = "-Y";

            plusX.transform.position = Vector3.right * m_uiDistance;
            plusX.transform.rotation = Quaternion.LookRotation(Vector3.right);
            plusX.Text = "+X";

            minusX.transform.position = Vector3.left * m_uiDistance;
            minusX.transform.rotation = Quaternion.LookRotation(Vector3.left);
            minusX.Text = "-X";
        }

        private VRRecenterCtrl InitCtrl()
        {
            VRRecenterCtrl ctrl = Instantiate(m_recenterCtrlPrefab, transform);
            Canvas canvas = ctrl.GetComponentInChildren<Canvas>();
            if(canvas != null && Component.Window != null)
            {
                canvas.worldCamera = Component.Window.Camera;
            }
            ctrl.name = "Ctrl";
            ctrl.Recenter += OnRecenter;
            ctrl.Up += OnUp;
            ctrl.Down += OnDown;
            ctrl.Left += OnLeft;
            ctrl.Right += OnRight;
            return ctrl;
        }

     
        private void OnRight(object sender, System.EventArgs e)
        {
            if (Recenter != null)
            {
                Quaternion rotation = GetRotation(sender, Vector3.right);
                Recenter(rotation);
            }
        }

        private void OnLeft(object sender, System.EventArgs e)
        {
            if (Recenter != null)
            {
                Quaternion rotation = GetRotation(sender, Vector3.left);
                Recenter(rotation);
            }
        }

        private void OnDown(object sender, System.EventArgs e)
        {
            if (Recenter != null)
            {
                Quaternion rotation = GetRotation(sender, Vector3.down);
                Recenter(rotation);
            }
        }

        private void OnUp(object sender, System.EventArgs e)
        {
            if(Recenter != null)
            {
                Quaternion rotation = GetRotation(sender, Vector3.up);
                Recenter(rotation);
            }
        }

        private void OnRecenter(object sender, System.EventArgs e)
        {
            if(Recenter != null)
            {
                Quaternion rotation = GetRotation(sender);
                Recenter(rotation);
            }
        }

        private Quaternion GetRotation(object sender)
        {
            VRRecenterCtrl ctrl = (VRRecenterCtrl)sender;
            Quaternion rotation = Quaternion.LookRotation((ctrl.transform.position - transform.position).normalized);
            return rotation;
        }

        private Quaternion GetRotation(object sender, Vector3 vector)
        {
            VRRecenterCtrl ctrl = (VRRecenterCtrl)sender;
            Quaternion rotation = Quaternion.LookRotation((ctrl.transform.position - transform.position).normalized);

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            vector = matrix.MultiplyVector(vector);
            rotation = Quaternion.LookRotation(vector);
            return rotation;
        }

        private void Update()
        {
            transform.position = Window.Camera.transform.position;
        }
    }

}
