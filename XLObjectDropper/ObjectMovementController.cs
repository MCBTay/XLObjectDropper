using GameManagement;
using Rewired;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UserInterface;

namespace XLObjectDropper
{
	public class ObjectMovementController : MonoBehaviour
	{
		public GameObject PreviewObject { get; set; }
        public List<GameObject> SpawnedObjects { get; set; }

		public GameObject ControlLegendGameObject { get; set; }

		private void Awake()
        {
	        SpawnedObjects = new List<GameObject>();

	        var position = new Vector3(GameStateMachine.Instance.PinObject.transform.position.x, groundLevel, GameStateMachine.Instance.PinObject.transform.position.z);
	        PreviewObject = Instantiate(AssetBundleHelper.LoadedAssets.ElementAt(0), position, GameStateMachine.Instance.PinObject.transform.rotation);
	        PreviewObject.SetActive(false);
	        DontDestroyOnLoad(PreviewObject);

	        ControlLegendGameObject = AssetBundleHelper.LoadUIBundle();

	        if (!(GameStateMachine.Instance.CurrentState.GetType() != typeof(ObjectMovementState)))
				return;

			enabled = false;
        }

        private void OnEnable()
        {
	        enabled = true;
	        GameStateMachine.Instance.PinObject.SetActive(true);

			ControlLegendGameObject?.SetActive(true);

			PreviewObject.SetActive(true);
        }

        private void OnDisable()
        {
			enabled = false;
			GameStateMachine.Instance.PinObject.SetActive(false);

			ControlLegendGameObject?.SetActive(false);

			PreviewObject.SetActive(false);
		}

        private bool showMenu;

        private void Update()
        {
	        Time.timeScale = 1.0f;

	        UpdateGroundLevel();

	        var position = new Vector3(GameStateMachine.Instance.PinObject.transform.position.x, groundLevel, GameStateMachine.Instance.PinObject.transform.position.z);
	        PreviewObject.transform.position = position;
			//PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;

			Player player = PlayerController.Instance.inputController.player;

			if (player.GetButtonSinglePressHold("LB"))
			{
				Time.timeScale = 0.0f;

				Debug.Log("XLObjectDropper: Holding LB");

				// If left stick movement, rotate the object on X/z axis
				Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");
				//PreviewObject.transform.eulerAngles = new Vector3(0, Mathf.Atan2(leftStick.y, leftStick.x) * 180 / Mathf.PI, 0);
				PreviewObject.transform.Rotate(leftStick.y, leftStick.x, 0);

				// If right stick Y movement, scale object
				Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");
				PreviewObject.transform.localScale += new Vector3(rightStick.y / 10, rightStick.y / 10, rightStick.y / 10);


				// If a, place the object, but keep the preview object
				if (player.GetButtonDown("A"))
				{
					Debug.Log("XLObjectDropper: Holding LB Pressed A");

					UISounds.Instance?.PlayOneShotSelectMajor();

					var newObject = Instantiate(PreviewObject, PreviewObject.transform.position, PreviewObject.transform.rotation);
					newObject.SetActive(true);
					SpawnedObjects.Add(newObject);
				}
			}
			else
	        {
		        // If dpad up/down, move object up/down
				// If dpad left/right, zoom cam in/out
		        Vector2 dpad = player.GetAxis2D("DPadX", "DPadY");
		        PreviewObject.transform.position = new Vector3(PreviewObject.transform.position.x, PreviewObject.transform.position.y + dpad.y, PreviewObject.transform.position.z);

		  //      var minFov = 15f;
		  //      var maxFov = 15f;
		  //      var sensitivity = 10f;
		  //      var fov = Camera.main.fieldOfView;
		  //      fov += dpad.y * sensitivity;
		  //      fov = Mathf.Clamp(fov, minFov, maxFov);
				//Camera.main.fieldOfView = fov;


		        if (player.GetButtonDown("A"))
				{
					// If a, place object and delete preview
					Debug.Log("XLObjectDropper: Pressed A");
					UISounds.Instance?.PlayOneShotSelectMajor();

					var newObject = Instantiate(PreviewObject, PreviewObject.transform.position, PreviewObject.transform.rotation);
					newObject.SetActive(true);
					SpawnedObjects.Add(newObject);

					PreviewObject.SetActive(false);
				}
				else if (player.GetButtonDown("X"))
				{
					// if x, open new object selection menu
					Debug.Log("XLObjectDropper: Pressed X");
					UISounds.Instance?.PlayOneShotSelectMinor();
					
					showMenu = !showMenu;

					GameStateMachine.Instance.RequestTransitionTo(typeof(ObjectSelectionState));
				}
				else if (player.GetButtonDown("Y"))
				{
					// if y, delete highlighted object (if any)
					Debug.Log("XLObjectDropper: Pressed Y");
				}
	        }
        }

        private float groundLevel;

        private bool UpdateGroundLevel()
        {
	        Ray ray1 = new Ray(this.transform.position, Vector3.down);
	        Ray ray2 = new Ray(this.transform.position, Vector3.down);
	        RaycastHit raycastHit = new RaycastHit();
	        ref RaycastHit local = ref raycastHit;
	        if (!Physics.SphereCast(ray1, 0.2f, out local))
		        return false;
	        this.groundLevel = raycastHit.point.y;

			Debug.Log("XLObjectDropper: groundLevel : " + groundLevel);

	        return true;
        }
	}
}
