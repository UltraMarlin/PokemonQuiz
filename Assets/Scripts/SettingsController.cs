using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject playerGrid;
    [SerializeField] private TMP_Dropdown playerCountDropdown;
    // Start is called before the first frame update
    void Start()
    {
        playerCountDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(playerCountDropdown); });
        DropdownValueChanged(playerCountDropdown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        int playerCount = dropdown.value + 1;
        for (int i = 0; i < 8; i++)
        {
            playerGrid.transform.GetChild(i).gameObject.SetActive(i < playerCount);
        }
    }
}
