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

    private RawImage blurImageComponent;
    private bool distortionActive;
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
        distortionActive = true;
        StartCoroutine(ReduceDistortionOverTime(1.0f, 0.0f, 30.0f));
    }

    public void NextQuestionStep()
    {
        return;
    }

    public void ShowSolution()
    {
        distortionActive = false;
        blurImageComponent.texture = blurQuestionData.sprite.texture;
        solutionText.text = blurQuestionData.pokemonName;
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
            elapsedTime += Time.deltaTime;
            currentDistortion = Mathf.Lerp(initialStrength, finalStrength, elapsedTime / duration);
            distortionMaterial.SetFloat("_DistortionAmount", currentDistortion);

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
    }
}
