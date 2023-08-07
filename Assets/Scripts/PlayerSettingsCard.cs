using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsCard : MonoBehaviour
{
    [SerializeField] private Image panelBackground;
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private List<Color> playerPanelColors = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        colorDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(colorDropdown); });
        DropdownValueChanged(colorDropdown);
    }

    public void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        panelBackground.color = playerPanelColors[dropdown.value];
    }
}
