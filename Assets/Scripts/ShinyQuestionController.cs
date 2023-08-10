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
    public int solutionIndex;

    private Color fakeBackgroundColor = new Color(255f / 255, 108f / 255, 97f / 255, 1.0f); //rgb(255, 108, 97)
    private Color solutionBackgroundColor = new Color(97f / 255, 255f / 255, 101f / 255, 1.0f); //rgb(97, 255, 101)

    public void SetData(IQuestion questionData)
    {
        shinyQuestionData = questionData as ShinyQuestion;
    }

    public void StartQuestion()
    {
        if (shinyQuestionData == null) return;

        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Empty);

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
