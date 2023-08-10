using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;
    [SerializeField] private TextMeshProUGUI solutionText;

    public FeatureQuestion featureQuestionData;
    private int currentFeatureIndex = 0;
    private bool lastFeatureShown = false;

    public void SetData(IQuestion questionData)
    {
        featureQuestionData = questionData as FeatureQuestion;
    }

    public void StartQuestion()
    {
        if (featureQuestionData == null) return;
        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.NextStep);
        AddSpriteToContainer(featureQuestionData.featureSprites[currentFeatureIndex]);
    }

    public void NextQuestionStep()
    {
        if (lastFeatureShown) return;
        currentFeatureIndex++;
        AddSpriteToContainer(featureQuestionData.featureSprites[currentFeatureIndex]);

        if (currentFeatureIndex == featureQuestionData.featureSprites.Count - 1)
        {
            lastFeatureShown = true;
            QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Empty);
        }
    }

    public void ShowSolution()
    {
        AddSpriteToContainer(featureQuestionData.solutionSprite);
        solutionText.text = featureQuestionData.pokemonName;
    }

    public void ResetDisplay()
    {
        foreach (Transform transform in imageContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    public void ToggleBackground()
    {
        Image bgImage = imageContainer.GetComponent<Image>();
        bgImage.enabled = !bgImage.enabled;
    }

    public void AddSpriteToContainer(Sprite sprite)
    {
        GameObject featureImage = Instantiate(stretchImagePrefab, imageContainer.transform);
        featureImage.GetComponent<Image>().sprite = sprite;
    }
}
