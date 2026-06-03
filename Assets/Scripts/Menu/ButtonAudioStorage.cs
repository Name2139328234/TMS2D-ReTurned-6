using UnityEngine;



public class ButtonAudioStorage
{
    private AudioSource _audioSource;



    public ButtonAudioStorage(AudioSource audioSource)
    {
        _audioSource = audioSource;
    }
    public void Play()
    {
        _audioSource.Play();
    }
}
