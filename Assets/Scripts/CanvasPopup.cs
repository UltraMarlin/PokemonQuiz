using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasPopup : MonoBehaviour
{
    public Canvas parentCanvas;
    public TMP_Text popupText;
    public bool isReady = false;
    public bool showFromStart;
    public bool clearFromStart;

    private void Start()
    {
        if (clearFromStart)
        {
            SetPopupText("");
        }
        if (showFromStart)
        {
            ShowPopup();
        }
        else
        {
            HidePopup();
        }
        isReady = true;
    }

    public void HidePopup()
    {
        parentCanvas.enabled = false;
    }

    public void ShowPopup()
    {
        parentCanvas.enabled = true;
    }

    public void SetPopupText(string infoString)
    {
        popupText.text = infoString;
    }
}