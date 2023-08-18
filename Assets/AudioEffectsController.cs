using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    Buzzer,
    Correct,
    Wrong,
}
public class AudioEffectsController : MonoBehaviour
{
    [SerializeField] private AudioSource buzzerHitAudioSource;
    [SerializeField] private AudioSource correctAnswerAudioSource;
    [SerializeField] private AudioSource wrongAnswerAudioSource;

    public void Play(SoundEffect effect)
    {
        if (effect == SoundEffect.Buzzer)
        {
            if (!buzzerHitAudioSource.isPlaying) buzzerHitAudioSource.Play();
        }
        else if (effect == SoundEffect.Correct)
        {
            if (!correctAnswerAudioSource.isPlaying) correctAnswerAudioSource.Play();
        }
        else if (effect == SoundEffect.Wrong)
        {
            if (!wrongAnswerAudioSource.isPlaying) wrongAnswerAudioSource.Play();
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
}
