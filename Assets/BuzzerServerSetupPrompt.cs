using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuzzerServerSetupPrompt : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomNumberInput;
    [SerializeField] private TMP_InputField adminPasswordInput;
    [SerializeField] private TMP_Text roomNumberPlaceholder;
    [SerializeField] private TMP_Text adminPasswordPlaceholder;
    [SerializeField] private TMP_Text infoMessageText;
    [SerializeField] private TMP_Text roomNumberHideButtonText;
    [SerializeField] private TMP_Text adminPasswordHideButtonText;
    private bool roomNumberHidden = false;
    private bool adminPasswordHidden = true;

    public void Start()
    {
        roomNumberInput.text = QuizSession.instance.buzzerRoomCode;
        adminPasswordInput.text = QuizSession.instance.buzzerRoomAdminPassword;
        UpdateRoomNumberHiddenState();
        UpdateAdminPasswordHiddenState();
    }

    public void ConnectToBuzzerServer()
    {
        DisplayInfoMessage("");
        if (roomNumberInput.text.Length < 4)
        {
            DisplayInfoMessage("Room Number must be at least 4 characters long.");
            return;
        }
        QuizSession.instance.ClearQuestionContainer();
        QuizSession.instance.SetUpSocketIOConnection(roomNumberInput.text, adminPasswordInput.text);
    }

    public void DisplayInfoMessage(string message)
    {
        infoMessageText.text = message;
    }

    public void ToggleRoomNumberHidden()
    {
        roomNumberHidden = !roomNumberHidden;
        UpdateRoomNumberHiddenState();
    }

    public void UpdateRoomNumberHiddenState()
    {
        roomNumberHideButtonText.text = roomNumberHidden ? "Show" : "Hide";
        roomNumberInput.contentType = roomNumberHidden ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Alphanumeric;
        roomNumberInput.pointSize = roomNumberHidden ? 52 : 38;
        roomNumberPlaceholder.fontSize = 26;
    }

    public void ToggleAdminPasswordHidden() {
        adminPasswordHidden = !adminPasswordHidden;
        UpdateAdminPasswordHiddenState();
    }

    public void UpdateAdminPasswordHiddenState()
    {
        adminPasswordHideButtonText.text = adminPasswordHidden ? "Show" : "Hide";
        adminPasswordInput.contentType = adminPasswordHidden ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Alphanumeric;
        adminPasswordInput.pointSize = adminPasswordHidden ? 38 : 28;
        adminPasswordPlaceholder.fontSize = 26;
    }

    public void GenerateRandomRoomNumber()
    {
        roomNumberInput.text = GenerateRoomCode(4, true);
    }

    public void GenerateRandomAdminPasswort()
    {
        adminPasswordInput.text = GenerateRoomCode(12, false);
    }

    public string GenerateRoomCode(int length, bool fiftySixEasteregg)
    {
        string chars56 = "ABCDEFGHJKLMNPQRSTUVWXYZ1234565656565656565656565656656789";
        string charsPassword = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ123456789";

        string chars = fiftySixEasteregg ? chars56 : charsPassword;

        char[] stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return new string(stringChars);
    }
}
