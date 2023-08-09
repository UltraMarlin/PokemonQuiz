using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
