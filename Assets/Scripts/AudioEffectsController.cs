using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    Buzzer,
    Correct,
    Wrong,
    Ticking,
    TimerOver
}

public class AudioEffectsController : MonoBehaviour
{
    [SerializeField] private AudioSource buzzerHitAudioSource;
    [SerializeField] private AudioSource correctAnswerAudioSource;
    [SerializeField] private AudioSource wrongAnswerAudioSource;
    [SerializeField] private AudioSource tickingAudioSource;
    [SerializeField] private AudioSource timerOverAudioSource;

    public void Play(SoundEffect effect)
    {
        if (effect == SoundEffect.Buzzer)
        {
            buzzerHitAudioSource.Play();
        }
        else if (effect == SoundEffect.Correct)
        {
            correctAnswerAudioSource.Play();
        }
        else if (effect == SoundEffect.Wrong)
        {
            wrongAnswerAudioSource.Play();
        }
        else if (effect == SoundEffect.Ticking)
        {
            tickingAudioSource.Play();
        }
        else if (effect == SoundEffect.TimerOver)
        {
            timerOverAudioSource.Play();
        }
    }

    public void PlayBuzzerSound()
    {
        Play(SoundEffect.Buzzer);
    }

    public void PlayCorrectSound()
    {
        Play(SoundEffect.Correct);
    }

    public void PlayWrongSound()
    {
        Play(SoundEffect.Wrong);
    }
    
    public void StopTicking()
    {
        tickingAudioSource?.Stop();
    }
}
