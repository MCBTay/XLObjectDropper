using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityModManagerNet;

public class OptionsMenu : MonoBehaviour
{
	[Header("Options Menu Elements")]
	public GameObject SnappingToggle;

	public GameObject SensitivitySlider;
	public GameObject SaveButton;
	public GameObject UndoButton;
	public GameObject RedoButton;
	public GameObject LoadButton;

	void OnSaveButtonClick()
	{

	}

	void Awake()
	{
		var button = SaveButton.GetComponent<Button>();

		button.onClick.AddListener(delegate { UnityModManager.Logger.Log("XLMenuMod: You clicked save!"); });
	}

	// Update is called once per frame
	void Update()
	{

	}
}
