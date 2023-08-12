using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplainScreenController : MonoBehaviour
{
    [SerializeField] private List<Sprite> explainImages;
    [SerializeField] private Image explainImage;

    public void ExplainQuestionType(QuestionType type)
    {
        explainImage.enabled = true;
        explainImage.sprite = explainImages[(int)type];
    }
}
