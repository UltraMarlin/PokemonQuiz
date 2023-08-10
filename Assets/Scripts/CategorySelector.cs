using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategorySelector : MonoBehaviour
{
    private List<Button> buttons = new();
    private List<CategoryButtonController> buttonControllers = new();
    [SerializeField] private GameObject buttonParent;

    void Start()
    {
        foreach (Transform child in buttonParent.transform)
        {
            Button button = child.GetComponent<Button>();
            buttons.Add(button);
            buttonControllers.Add(button.GetComponent<CategoryButtonController>());
        }
    }

    public void SetActiveCategoryLabel(int toggleIndex)
    {
        if (toggleIndex < 0) return;
        Debug.Log("Set Active Category Label: " + toggleIndex);
        for (int i = 0; i < buttonControllers.Count; i++)
        {
            if (i == toggleIndex)
            {
                buttonControllers[i].SetHighlight();
            } else
            {
                buttonControllers[i].ResetHighlight();
            }
        }
        
    }

    public void ButtonActivated(int toggleIndex)
    {
        if (AdminPanelController.instance.adminPanelUser == null || AdminPanelController.instance.quizOver)
            return;
        AdminPanelController.instance.adminPanelUser.SendCategorySwitchServerRpc(toggleIndex);
    }
}
