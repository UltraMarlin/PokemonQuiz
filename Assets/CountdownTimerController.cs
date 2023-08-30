using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimerController : MonoBehaviour
{
    [SerializeField] private Image circleProgress;
    [SerializeField] private TMP_Text countdownText;
    private float initialTime = 1f;
    private float timeLeft = 0f;
    private bool timerStarted = false;
    private bool timeOverSoundPlayed = false;

    private void Start()
    {
        countdownText.text = string.Empty;
    }

    void Update()
    {
        if (!timerStarted) return;

        timeLeft -= Time.deltaTime;

        countdownText.text = Mathf.Ceil(timeLeft).ToString();
        circleProgress.fillAmount = timeLeft / initialTime;

        if (!timeOverSoundPlayed && timeLeft < 0f)
        {
            timeOverSoundPlayed = true;
            QuizSession.instance.CountdownTimerOver();
            StartCoroutine(DestroyAfterDelay(0.5f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void StartCountdown(float time)
    {
        timerStarted = true;
        timeLeft = time;
        initialTime = time;
    }
}
