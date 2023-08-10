using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FootprintQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject stretchImagePrefab;
    [SerializeField] private QuestionDB footprintQuestionDB;

    [SerializeField] private GameObject referenceImageContainer;
    [SerializeField] private List<GameObject> optionsImageContainerList;
    [SerializeField] private List<Image> optionsImageBackgroundList;
    [SerializeField] private TextMeshProUGUI pokemonName;

    public FootprintQuestion footprintQuestionData;
    public int solutionIndex;

    private Color fakeBackgroundColor = new Color(255f / 255, 108f / 255, 97f / 255, 1.0f); //rgb(255, 108, 97)
    private Color solutionBackgroundColor = new Color(97f / 255, 255f / 255, 101f / 255, 1.0f); //rgb(97, 255, 101)

    public void SetData(IQuestion questionData)
    {
        footprintQuestionData = questionData as FootprintQuestion;
    }

    public void StartQuestion()
    {
        if (footprintQuestionData == null) return;

        QuizSession.instance.SetNextStepButtonTextId(NextStepButtonState.Empty);

        pokemonName.text = footprintQuestionData.pokemonName;
        AddSpriteToContainer(footprintQuestionData.portraitSprite, referenceImageContainer);

        solutionIndex = Random.Range(0, optionsImageContainerList.Count);

        List<Sprite> fakeFootprints = new List<Sprite>();
        List<int> excludedGroups = footprintQuestionData.footprintGroups.ToList();
        for (int i = 0; i < 3; i++)
        {
            FootprintQuestion questionNotExcludedByFootprintGroups = footprintQuestionDB.questions.ToList().Where(question => {
                foreach (int group in (question as FootprintQuestion).footprintGroups)
                {
                    if (excludedGroups.Contains(group))
                        return false;
                }
                return true;
            }).OrderBy(x => UnityEngine.Random.value).ToList()[0] as FootprintQuestion;

            foreach (int group in questionNotExcludedByFootprintGroups.footprintGroups)
            {
                excludedGroups.Add(group);
            }
            fakeFootprints.Add(questionNotExcludedByFootprintGroups.footprintSprite);
        }

        int fakeIndex = 0;

        for (int i = 0; i < optionsImageContainerList.Count; i++)
        {
            if (i != solutionIndex)
            {
                AddSpriteToContainer(fakeFootprints[fakeIndex], optionsImageContainerList[i]);
                fakeIndex++;
            }
            else
            {
                AddSpriteToContainer(footprintQuestionData.footprintSprite, optionsImageContainerList[i]);
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
            }
            else
            {
                optionsImageBackgroundList[i].color = solutionBackgroundColor;
            }
        }
        if (referenceImageContainer.transform.childCount > 0)
            Destroy(referenceImageContainer.transform.GetChild(0).gameObject);
        AddSpriteToContainer(footprintQuestionData.solutionSprite, referenceImageContainer);
        (referenceImageContainer.transform as RectTransform).sizeDelta = new Vector2(500f, 500f);
        LayoutRebuilder.MarkLayoutForRebuild(referenceImageContainer.transform as RectTransform);
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
