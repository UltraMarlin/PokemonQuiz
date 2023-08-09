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

    private Color fakeBackgroundColor = new Color(1f, 0f, 0f, 0.3f);
    private Color solutionBackgroundColor = new Color(0f, 1f, 0f, 0.3f);

    public void SetData(IQuestion questionData)
    {
        footprintQuestionData = questionData as FootprintQuestion;
    }

    public void StartQuestion()
    {
        if (footprintQuestionData == null) return;

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
