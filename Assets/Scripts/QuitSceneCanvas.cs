using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitSceneCanvas : CanvasPopup
{
    void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            if (parentCanvas.enabled)
            {
                HidePopup();
            }
            else
            {
                SetPopupText("Zum Hauptmenü?");
                ShowPopup();
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}