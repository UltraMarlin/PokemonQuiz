using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPanelController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image highlightImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI textFieldText;
    [SerializeField] private List<Sprite> playerPanelSprites = new List<Sprite>();
    [SerializeField] private List<Color> playerTextColors = new List<Color>();
    private int MAX_TEXT_LENGTH = 8;
    private float minDelay = 0.8f;
    private float maxDelay = 8.0f;
    private int spriteIndex1, spriteIndex2;
    private bool blockRandomAnimation = false;

    public void Start()
    {
        highlightImage.enabled = false;
    }

    public void SetPlayerColor(PlayerColor playerColor)
    {
        spriteIndex1 = (int)playerColor * 2;
        spriteIndex2 = spriteIndex1 + 1;
        backgroundImage.sprite = playerPanelSprites[spriteIndex1];
        playerNameText.color = playerTextColors[(int)playerColor];
        pointsText.color = playerTextColors[(int)playerColor];
        textFieldText.color = playerTextColors[(int)playerColor];
        StartCoroutine(PlayAnimationWithRandomDelay(minDelay, maxDelay));
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
        if (!blockRandomAnimation)
            backgroundImage.sprite = playerPanelSprites[spriteIndex2];
        StartCoroutine(SwitchToNormalSpriteAfterSeconds(duration));
    }

    public void SetTextFieldText(string text)
    {
        textFieldText.text = text.Substring(0, Mathf.Min(text.Length, MAX_TEXT_LENGTH));
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
        if (!blockRandomAnimation)
            backgroundImage.sprite = playerPanelSprites[spriteIndex1];
    }

    public IEnumerator PlayWinAnimation()
    {
        blockRandomAnimation = true;
        for (int i = 0; i < 4; i++)
        {
            backgroundImage.sprite = playerPanelSprites[spriteIndex2];
            yield return new WaitForSeconds(0.15f);
            backgroundImage.sprite = playerPanelSprites[spriteIndex1];
            yield return new WaitForSeconds(0.15f);
        }
        blockRandomAnimation = false;
    }
}
