using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Coffee.UIEffects;

public class PlayerPanelController : MonoBehaviour
{
    [SerializeField] private Image highlightImage;
    [SerializeField] private UIShadow highlightShadow1, highlightShadow2;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI textFieldText;
    [SerializeField] private List<Sprite> playerPanelSprites = new List<Sprite>();
    [SerializeField] private List<Color> playerTextColors = new List<Color>();
    [SerializeField] private List<Color> playerPanelColors = new List<Color>();
    [SerializeField] private List<Color> playerGlowColors = new List<Color>();
    private int MAX_TEXT_LENGTH = 8;
    private float minDelay = 0.8f;
    private float maxDelay = 8.0f;
    private int spriteIndex1, spriteIndex2;
    private int currentIndex;
    private bool blockRandomAnimation = false;
    private bool winningAnimation = false;
    private string currentText = "";
    private PlayerColor color;

    public void Start()
    {
        highlightImage.enabled = false;
    }

    public void SetPlayerColor(PlayerColor playerColor)
    {
        color = playerColor;
        spriteIndex1 = (int)playerColor * 2;
        spriteIndex2 = spriteIndex1 + 1;
        currentIndex = spriteIndex1;
        playerNameText.color = playerTextColors[(int)playerColor];
        pointsText.color = playerTextColors[(int)playerColor];
        textFieldText.color = playerTextColors[(int)playerColor];
        bool previousHighlightImageEnabled = highlightImage.enabled;
        highlightImage.enabled = true;

        UpdateSprite();
        highlightImage.color = playerGlowColors[(int)playerColor];
        highlightShadow1.effectColor = playerGlowColors[(int)playerColor];
        highlightShadow2.effectColor = playerGlowColors[(int)playerColor];

        highlightImage.enabled = previousHighlightImageEnabled;
        StartCoroutine(PlayAnimationWithRandomDelay(minDelay, maxDelay));
    }

    public Color GetPlayerTextColor()
    {
        return playerTextColors[(int)color];
    }

    public Color GetPlayerPanelColor()
    {
        return playerPanelColors[(int)color];
    }

    public void SetPlayerNameText(string text)
    {
        playerNameText.text = text;
    }

    public void SetPlayer(Player player)
    {
        SetPlayerColor(player.color);
        SetPlayerNameText(player.name);
    }

    public void SetPlayerPointsText(string text)
    {
        pointsText.text = text;
    }

    public string GetPlayerName()
    {
        return playerNameText.text;
    }

    private IEnumerator PlayAnimationWithRandomDelay(float minDelay,  float maxDelay)
    {
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        PlayAnimation(0.2f);
        StartCoroutine(PlayAnimationWithRandomDelay(minDelay, maxDelay));
    }

    public void PlayAnimation(float duration)
    {
        currentIndex = spriteIndex2;
        if (!blockRandomAnimation)
            UpdateSprite();
        StartCoroutine(SwitchToNormalSpriteAfterSeconds(duration));
    }

    public void SetTextFieldText(string text, bool update)
    {
        currentText = text.Substring(0, Mathf.Min(text.Length, MAX_TEXT_LENGTH));
        if (update) ReloadTextFieldTexts();
    }

    public void ShowTextField(bool show)
    {
        textFieldText.enabled = show;
        ReloadTextFieldTexts();
    }

    public void ReloadTextFieldTexts()
    {
        if (textFieldText.isActiveAndEnabled)
            textFieldText.text = currentText.ToUpper();
    }

    public void SetBuzzerHighlight()
    {
        Debug.Log("Set Buzzer Highlight: " + playerNameText.text);
        highlightImage.enabled = true;
        Debug.Log(highlightImage.enabled);
        blockRandomAnimation = true;
    }

    public void ResetBuzzerHighlight()
    {
        highlightImage.enabled = false;
        blockRandomAnimation = false;
    }

    private IEnumerator SwitchToNormalSpriteAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentIndex = spriteIndex1;
        if (!blockRandomAnimation)
            UpdateSprite();
    }

    public IEnumerator PlayCorrectAnimation()
    {
        blockRandomAnimation = true;
        for (int i = 0; i < 4; i++)
        {
            currentIndex = spriteIndex2;
            UpdateSprite();
            yield return new WaitForSeconds(0.15f);
            currentIndex = spriteIndex1;
            UpdateSprite();
            yield return new WaitForSeconds(0.15f);
        }
        blockRandomAnimation = false;
    }

    public IEnumerator PlayWinAnimation()
    {
        blockRandomAnimation = true;
        winningAnimation = true;
        while (winningAnimation)
        {
            currentIndex = spriteIndex2;
            UpdateSprite();
            yield return new WaitForSeconds(0.15f);
            currentIndex = spriteIndex1;
            UpdateSprite();
            yield return new WaitForSeconds(0.15f);
        }
        blockRandomAnimation = false;
    }

    public void StopWinningAnimation()
    {
        winningAnimation = false;
    }

    public void UpdateSprite()
    {
        backgroundImage.sprite = playerPanelSprites[currentIndex];
        if (highlightImage.isActiveAndEnabled)
            highlightImage.sprite = playerPanelSprites[currentIndex];
    }
}
