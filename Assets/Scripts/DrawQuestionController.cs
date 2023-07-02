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
    [SerializeField] private Sprite blankSquare;
    [SerializeField] private RenderTexture videoRenderTexture;

    private VideoPlayer drawVideoPlayerComponent;
    private RawImage rawImageComponent;
    public DrawQuestion drawQuestionData;
    public bool videoDonePlaying;

    public void SetData(IQuestion questionData)
    {
        drawQuestionData = questionData as DrawQuestion;
    }

    public void Update()
    {
        if (videoDonePlaying) return;
        long framesLeft = (long)drawVideoPlayerComponent.frameCount - drawVideoPlayerComponent.frame;
        Debug.Log(framesLeft);
        if (framesLeft < 5 && !drawVideoPlayerComponent.isPlaying)
        {
            videoDonePlaying = true;
            drawVideoPlayerComponent.Play();
        }
    }

    public void StartQuestion()
    {
        drawerTitle.enabled = false;
        if (drawQuestionData == null) return;
        GameObject drawVideo = Instantiate(stretchVideoPrefab, videoContainer.transform);
        drawVideoPlayerComponent = drawVideo.GetComponent<VideoPlayer>();
        rawImageComponent = drawVideo.GetComponent<RawImage>();
        drawVideoPlayerComponent.waitForFirstFrame = true;
        drawVideoPlayerComponent.clip = drawQuestionData.videoClip;
        Graphics.Blit(blankSquare.texture, videoRenderTexture);
    }

    public void NextQuestionStep()
    {
        if (videoDonePlaying) return;
        if (drawVideoPlayerComponent == null) return;

        if (drawVideoPlayerComponent.isPlaying)
        {
            Debug.Log("Now Paused");
            drawVideoPlayerComponent.Pause();
        } else
        {
            Debug.Log("Now Playing");
            drawVideoPlayerComponent.Play();
        }
    }

    public void ShowSolution()
    {
        solutionText.text = drawQuestionData.pokemonName;
        drawerTitle.enabled = true;
        drawerText.text = drawQuestionData.drawerName;

        if (videoDonePlaying) return;
        videoDonePlaying = true;

        drawVideoPlayerComponent.frame = (long)drawVideoPlayerComponent.frameCount;
        if (!drawVideoPlayerComponent.isPlaying) drawVideoPlayerComponent.Play();
    }

    public void ResetDisplay()
    {

    }
}
