using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButtonController : MonoBehaviour
{
    [SerializeField] private int buttonID = -1;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image buttonImage;
    private CategorySelector categorySelector;

    // Start is called before the first frame update
    void Start()
    {
        buttonID = transform.GetSiblingIndex();
        categorySelector = GetComponentInParent<CategorySelector>();
    }

    public void SetHighlight()
    {
        buttonImage.color = new Color(104/255f, 197/255f, 79/255f); // rgb(104, 197, 79)
    }

    public void ResetHighlight()
    {
        buttonImage.color = Color.white;
    }

    public void ButtonClickHandler()
    {
        if (categorySelector != null && buttonID > -1)
        {
            categorySelector.ButtonActivated(buttonID);
            categorySelector.SetActiveCategoryLabel(buttonID);
        }
    }
}
