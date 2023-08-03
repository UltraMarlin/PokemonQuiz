using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategorySelector : MonoBehaviour
{
    private List<Button> buttons = new();
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private TextMeshProUGUI activeCategoryText;

    void Start()
    {
        foreach (Transform child in buttonParent.transform)
        {
            buttons.Add(child.GetComponent<Button>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveCategoryLabel(int toggleIndex)
    {
        Button button = buttons[toggleIndex];
        string categoryString = button.gameObject.GetComponent<CategoryButtonController>().getLabel();
        activeCategoryText.text = "Active Category: " + categoryString;
    }

    public void ButtonActivated(int toggleIndex)
    {
        if (AdminPanelController.instance.adminPanelUser != null)
            AdminPanelController.instance.adminPanelUser.SendCategorySwitchServerRpc(toggleIndex);
    }

}
