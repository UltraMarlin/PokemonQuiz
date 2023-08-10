using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;
    [SerializeField] private GameObject profileImageContainer;
    [SerializeField] private TextMeshProUGUI solutionText;

    public TeamQuestion teamQuestionData;

    public void SetData(IQuestion questionData)
    {
        teamQuestionData = questionData as TeamQuestion;
    }

    public void StartQuestion()
    {
        if (teamQuestionData == null) return;
        AddSpriteToContainer(teamQuestionData.teamImage, imageContainer);
    }

    public void NextQuestionStep()
    {
        return;
    }

    public void ShowSolution()
    {
        if (profileImageContainer.transform.childCount > 0)
        {
            Destroy(profileImageContainer.transform.GetChild(0));
        }
        AddSpriteToContainer(teamQuestionData.profileImage, profileImageContainer);
        solutionText.text = teamQuestionData.pokemonName;
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
