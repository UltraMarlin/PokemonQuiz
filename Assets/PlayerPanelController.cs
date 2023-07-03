using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPanelController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private List<Sprite> playerPanelSprites = new List<Sprite>();
    [SerializeField] private List<Color> playerTextColors = new List<Color>();
    private float minDelay = 0.8f;
    private float maxDelay = 8.0f;
    private int spriteIndex1, spriteIndex2;

    public void SetPlayerColor(PlayerColor playerColor)
    {
        spriteIndex1 = (int)playerColor * 2;
        spriteIndex2 = spriteIndex1 + 1;
        backgroundImage.sprite = playerPanelSprites[spriteIndex1];
        playerNameText.color = playerTextColors[(int)playerColor];
        pointsText.color = playerTextColors[(int)playerColor];
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

    private IEnumerator PlayAnimationWithRandomDelay(float minDelay,  float maxDelay)
    {
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        PlayAnimation(0.2f);
        StartCoroutine(PlayAnimationWithRandomDelay(minDelay, maxDelay));
    }

    public void PlayAnimation(float duration)
    {
        backgroundImage.sprite = playerPanelSprites[spriteIndex2];
        StartCoroutine(SwitchToNormalSpriteAfterSeconds(duration));
    }

    private IEnumerator SwitchToNormalSpriteAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        backgroundImage.sprite = playerPanelSprites[spriteIndex1];
    }
}
