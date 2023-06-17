using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DrawQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject videoContainer;
    [SerializeField] private GameObject stretchVideoPrefab;
    [SerializeField] private TextMeshProUGUI solutionText;
    [SerializeField] private TextMeshProUGUI drawerTitle;
    [SerializeField] private TextMeshProUGUI drawerText;

    private VideoPlayer drawVideoPlayerComponent;
    public DrawQuestion drawQuestionData;

    public void SetData(IQuestion questionData)
    {
        drawQuestionData = questionData as DrawQuestion;
    }

    public void StartQuestion()
    {
        drawerTitle.enabled = false;
        if (drawQuestionData == null) return;
        GameObject drawVideo = Instantiate(stretchVideoPrefab, videoContainer.transform);
        drawVideoPlayerComponent = drawVideo.GetComponent<VideoPlayer>();
        drawVideoPlayerComponent.clip = drawQuestionData.videoClip;
    }

    public void NextQuestionStep()
    {
        if (drawVideoPlayerComponent == null) return;

        if (drawVideoPlayerComponent.isPlaying)
        {
            drawVideoPlayerComponent.Pause();
        } else
        {
            drawVideoPlayerComponent.Play();
        }
    }

    public void ShowSolution()
    {
        drawVideoPlayerComponent.Pause();
        drawVideoPlayerComponent.frame = (long)drawVideoPlayerComponent.frameCount - 1;
        solutionText.text = drawQuestionData.pokemonName;
        drawerTitle.enabled = true;
        drawerText.text = drawQuestionData.drawerName;
    }

    public void ResetDisplay()
    {
        drawVideoPlayerComponent.Pause();
        drawVideoPlayerComponent.Play();
    }
}
