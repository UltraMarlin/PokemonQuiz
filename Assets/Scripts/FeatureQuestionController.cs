using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;

    public FeatureQuestion featureQuestionData;
    private int currentFeatureIndex = 0;

    public void SetData(FeatureQuestion questionData)
    {
        featureQuestionData = questionData;
    }

    public void StartQuestion()
    {
        if (featureQuestionData == null) return;
        AddSpriteToContainer(featureQuestionData.featureSprites[currentFeatureIndex]);
    }

    public void NextQuestionStep()
    {
        if (currentFeatureIndex + 1 >= featureQuestionData.featureSprites.Count) return;
        currentFeatureIndex++;
        AddSpriteToContainer(featureQuestionData.featureSprites[currentFeatureIndex]);
    }

    public void ShowSolution()
    {
        AddSpriteToContainer(featureQuestionData.solutionSprite);
    }

    public void ResetDisplay()
    {
        foreach (Transform transform in imageContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    public void AddSpriteToContainer(Sprite sprite)
    {
        GameObject featureImage = Instantiate(stretchImagePrefab, imageContainer.transform);
        featureImage.GetComponent<Image>().sprite = sprite;
    }
}
