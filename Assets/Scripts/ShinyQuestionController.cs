using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShinyQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject stretchImagePrefab;

    [SerializeField] private GameObject referenceImageContainer;
    [SerializeField] private TextMeshProUGUI pokemonName;
    [SerializeField] private List<GameObject> optionsImageContainerList;
    [SerializeField] private List<Image> optionsImageBackgroundList;

    public ShinyQuestion shinyQuestionData;
    private int solutionIndex;

    private Color fakeBackgroundColor = new Color(1f, 0f, 0f, 0.3f);
    private Color solutionBackgroundColor = new Color (0f, 1f, 0f, 0.3f);

    public void SetData(IQuestion questionData)
    {
        shinyQuestionData = questionData as ShinyQuestion;
    }

    public void StartQuestion()
    {
        if (shinyQuestionData == null) return;

        pokemonName.text = shinyQuestionData.pokemonName;
        AddSpriteToContainer(shinyQuestionData.originalSprite, referenceImageContainer);
        
        solutionIndex = Random.Range(0, optionsImageContainerList.Count);
        List<Sprite> fakeSpritesCopy = shinyQuestionData.fakeSprites.ToList().OrderBy(x => Random.value).ToList();
        int fakeIndex = 0;

        for (int i = 0; i < optionsImageContainerList.Count; i++)
        {
            if (i != solutionIndex)
            {
                AddSpriteToContainer(fakeSpritesCopy[fakeIndex], optionsImageContainerList[i]);
                fakeIndex++;
            } else
            {
                AddSpriteToContainer(shinyQuestionData.solutionSprite, optionsImageContainerList[i]);
            }
        }

    }

    public void NextQuestionStep()
    {
        return;
    }

    public void ShowSolution()
    {
        for (int i = 0; i < optionsImageBackgroundList.Count; i++)
        {
            if (i != solutionIndex)
            {
                optionsImageBackgroundList[i].color = fakeBackgroundColor;
            } else
            {
                optionsImageBackgroundList[i].color = solutionBackgroundColor;
            }
        }
    }

    public void ResetDisplay()
    {
        return;
    }

    public void AddSpriteToContainer(Sprite sprite, GameObject container)
    {
        GameObject image = Instantiate(stretchImagePrefab, container.transform);
        image.GetComponent<Image>().sprite = sprite;
    }
}
