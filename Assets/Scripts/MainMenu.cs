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

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
        Application.targetFrameRate = 60;
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
}
