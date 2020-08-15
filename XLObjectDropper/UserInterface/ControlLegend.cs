using System.Linq;
using GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.GameManagement;

namespace XLObjectDropper.UserInterface
{
	public class ControlLegend : MonoBehaviour
	{
		public GameObject CanvasGameObject { get; set; }
		public Canvas Canvas { get; set; }
		public GameObject PanelGameObject { get; set; }

		private TMP_SpriteAsset ControllerIcons { get; set; }

		public GameObject ControlLedgendBackground { get; set; }

		private void Awake()
		{
			if (GameStateMachine.Instance.CurrentState.GetType() != typeof(ObjectMovementState))
				return;

			Debug.Log("XLObjectDropper: ControlLegend.Awake()");
			ControllerIcons = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>().FirstOrDefault(x => x.name == "ControllerIcons");

			CanvasGameObject = new GameObject("Canvas");
			DontDestroyOnLoad(CanvasGameObject);
			Canvas = CanvasGameObject.AddComponent<Canvas>();
			Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

			CanvasGameObject.AddComponent<CanvasScaler>();
			CanvasGameObject.AddComponent<GraphicRaycaster>();

			ControlLedgendBackground = new GameObject("ControlLegendBackground");
			Image i = ControlLedgendBackground.AddComponent<Image>();
			i.color = new Color(1, 1, 1, 0.25f);
			i.rectTransform.sizeDelta = new Vector2(500, 150);
			ControlLedgendBackground.transform.SetParent(CanvasGameObject.transform, false);
			ControlLedgendBackground.transform.position = new Vector3(Screen.width / 2, 100, 0);
			



			var abutton = CreateText(new Vector3(0, 0, 0), "Place Object", "A", Color.green);
			var bbutton = CreateText(new Vector3(0, -50, 0), "Exit", "B", Color.red);
			var ybutton = CreateText(new Vector3(200, 0, 0), "Delete", "Y", Color.yellow);
			var xbutton = CreateText(new Vector3(200, -50, 0), "Object Menu", "X", Color.blue);
			

			

			CanvasGameObject.SetActive(false);
			enabled = false;

			

			

			
		}

		private GameObject CreateText(Vector3 position, string text, string button, Color background)
		{
			var canvasRendererGameObject = new GameObject(text);
			canvasRendererGameObject.AddComponent<CanvasRenderer>();
			canvasRendererGameObject.transform.SetParent(ControlLedgendBackground.transform, false);
			canvasRendererGameObject.transform.localPosition = position;

			var labelGameObject = new GameObject();

			var m_Text = labelGameObject.AddComponent<TextMeshProUGUI>();
			m_Text.spriteAsset = ControllerIcons;
			m_Text.SetText($"<sprite={ControllerButtonHelper.GetSpriteIndex(button)}> {text}");
			m_Text.color = Color.black;
			m_Text.fontSize = 18;
			

			labelGameObject.transform.SetParent(canvasRendererGameObject.transform, false);
			

			return canvasRendererGameObject;
		}

		private void OnEnable()
		{
			CanvasGameObject?.SetActive(true);
			enabled = true;
		}

		private void OnDisable()
		{
			CanvasGameObject?.SetActive(false);
			enabled = false;
		}
	}
}
