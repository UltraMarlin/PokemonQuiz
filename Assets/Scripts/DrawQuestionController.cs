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
    [SerializeField] private GameObject originalImageContainer;
    [SerializeField] private GameObject stretchImagePrefab;

    private VideoPlayer drawVideoPlayerComponent;
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
        if (framesLeft < 5)
        {
            videoDonePlaying = true;
            QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Empty);
        }
    }

    public void StartQuestion()
    {
        drawerTitle.enabled = false;
        if (drawQuestionData == null) return;
        GameObject drawVideo = Instantiate(stretchVideoPrefab, videoContainer.transform);
        drawVideoPlayerComponent = drawVideo.GetComponent<VideoPlayer>();
        drawVideoPlayerComponent.waitForFirstFrame = true;
        drawVideoPlayerComponent.clip = drawQuestionData.videoClip;
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Pause);
        Graphics.Blit(blankSquare.texture, videoRenderTexture);
    }

    public void NextQuestionStep()
    {
        if (drawVideoPlayerComponent.isPlaying)
        {
            Pause();
        } else
        {
            Play();
        }
    }

    public void Pause()
    {
        if (videoDonePlaying || drawVideoPlayerComponent == null) return;
        Debug.Log("Now Paused");
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Play);
        drawVideoPlayerComponent.Pause();
    }

    public void Play()
    {
        if (videoDonePlaying || drawVideoPlayerComponent == null) return;
        Debug.Log("Now Playing");
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Pause);
        drawVideoPlayerComponent.Play();
    }

    public void ShowSolution()
    {
        solutionText.text = drawQuestionData.pokemonName;
        drawerTitle.enabled = true;
        drawerText.text = drawQuestionData.drawerName;
        if (originalImageContainer.transform.childCount > 0)
            Destroy(originalImageContainer.transform.GetChild(0).gameObject);
        AddSpriteToContainer(drawQuestionData.originalImage, originalImageContainer);

        if (videoDonePlaying) return;
        videoDonePlaying = true;
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Empty);

        drawVideoPlayerComponent.frame = (long)drawVideoPlayerComponent.frameCount;
        if (!drawVideoPlayerComponent.isPlaying) drawVideoPlayerComponent.Play();
    }

    public void AddSpriteToContainer(Sprite sprite, GameObject container)
    {
        GameObject image = Instantiate(stretchImagePrefab, container.transform);
        image.GetComponent<Image>().sprite = sprite;
    }

    public void ResetDisplay()
    {
        return;
    }
}
