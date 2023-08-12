using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlurQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;
    [SerializeField] private TextMeshProUGUI solutionText;
    [SerializeField] private List<Material> distortionEffects = new List<Material>();
    [SerializeField] private RenderTexture renderTextureA;
    [SerializeField] private RenderTexture renderTextureB;
    [SerializeField] private ProgressBar progressBar;

    private RawImage blurImageComponent;
    private bool distortionActive;
    private bool distortionPaused;
    private Material distortionMaterial;
    private int passCount;

    public float currentDistortion;
    public BlurQuestion blurQuestionData;

    public void SetData(IQuestion questionData)
    {
        blurQuestionData = questionData as BlurQuestion;
    }

    public void StartQuestion()
    {
        if (blurQuestionData == null) return;
        GameObject blurImage = Instantiate(stretchImagePrefab, imageContainer.transform);
        blurImageComponent = blurImage.GetComponent<RawImage>();
        blurImageComponent.texture = blurQuestionData.sprite.texture;
        distortionMaterial = distortionEffects[Random.Range(0, distortionEffects.Count)];
        passCount = distortionMaterial.passCount;
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Pause);
        distortionActive = true;
        StartCoroutine(ReduceDistortionOverTime(1.0f, 0.0f, 30.0f));
    }

    public void NextQuestionStep()
    {
        if (distortionPaused)
        {
            distortionPaused = false;
            QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Pause);
        } else
        {
            distortionPaused = true;
            QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Play);
        }
        Debug.Log("Paused: " + distortionPaused);
    }

    public void ShowSolution()
    {
        distortionActive = false;
        distortionPaused = false;
        solutionText.text = blurQuestionData.pokemonName;
        progressBar.gameObject.SetActive(false);
    }

    public void ResetDisplay()
    {
        return;
    }

    IEnumerator ReduceDistortionOverTime(float initialStrength, float finalStrength, float duration)
    {
        float elapsedTime = 0;
        currentDistortion = 1.0f;
        while (currentDistortion > 0.0f && distortionActive)
        {
            while (distortionPaused)
            {
                yield return null;
            }
            elapsedTime += Time.deltaTime;
            currentDistortion = Mathf.Lerp(initialStrength, finalStrength, elapsedTime / duration);
            distortionMaterial.SetFloat("_DistortionAmount", currentDistortion);
            if (progressBar.isActiveAndEnabled)
                progressBar.BarValue = Mathf.Lerp(0.8f, 99.2f, 1.0f - currentDistortion);

            for (int i = 0; i < passCount; i++)
            {
                if (i == 0)
                {
                    Graphics.Blit(blurQuestionData.sprite.texture, renderTextureA, distortionMaterial, i);
                }
                else
                {
                    if (i % 2 > 0)
                    {
                        Graphics.Blit(renderTextureA, renderTextureB, distortionMaterial, i);
                    }
                    else
                    {
                        Graphics.Blit(renderTextureB, renderTextureA, distortionMaterial, i);
                    }
                }
            }

            if (passCount % 2 == 0)
                blurImageComponent.texture = renderTextureB;
            else
                blurImageComponent.texture = renderTextureA;

            yield return new WaitForFixedUpdate();
        }
        distortionActive = false;
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Empty);
        blurImageComponent.texture = blurQuestionData.sprite.texture;
    }
}
