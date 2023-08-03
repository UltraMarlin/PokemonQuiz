using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButtonController : MonoBehaviour
{
    [SerializeField] private int buttonID = -1;
    [SerializeField] private TextMeshProUGUI text;
    private CategorySelector categorySelector;

    // Start is called before the first frame update
    void Start()
    {
        buttonID = transform.GetSiblingIndex();
        categorySelector = GetComponentInParent<CategorySelector>();
    }

    public string getLabel()
    {
        return text.text;
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
