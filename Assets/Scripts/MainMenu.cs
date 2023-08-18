using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text startButtonText;
    [SerializeField] private TMP_Text FPSButtonText;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
        if (Application.targetFrameRate < 0) Application.targetFrameRate = 60;
        FPSButtonText.text = $"FPS: {Application.targetFrameRate}";
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartQuiz()
    {
        startButtonText.text = "Ladevorgang...";
        SceneManager.LoadScene("PlayQuiz");
    }

    public void StartAdminPanel()
    {
        SceneManager.LoadScene("AdminPanel");
    }

    public void LoadQuizSelection()
    {
        SceneManager.LoadScene("QuizSelection");
    }

    public void FPSCycle()
    {
        if (Application.targetFrameRate < 20)
        {
            Application.targetFrameRate = 120;
        }
        else
        {
            Application.targetFrameRate -= 15;
        }
        
        FPSButtonText.text = $"FPS: {Application.targetFrameRate}";
    }
}
