using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Changed to GameObject because only the game object of the menu needs to be accessed, you can 
    // change this to any class that inherits MonoBehaviour
    public GameObject optionsMenu;
    bool cursorShow = false;

    // At step zero, disable the optionsMenu and hide the mouse cursor
    private void Awake()
    {
        optionsMenu.SetActive(false);
        Cursor.visible = (false);
    }

    // Update is called once per frame
    void Update()
    {
        // Reverse the active state every time F2 is pressed
        if (Input.GetKeyDown(KeyCode.F2))
        {
            optionsMenu.SetActive(!cursorShow);
            cursorShow = !cursorShow;

            if (optionsMenu.activeSelf)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
            }
        }

    }
}
