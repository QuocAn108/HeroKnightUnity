using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip musicClip;
    public AudioClip sfxClip;
    public AudioClip winClip;
    public AudioClip dieClip;
    public AudioClip swordClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    public void PlaySFX()
    {
        sfxAudioSource.clip = sfxClip;
        sfxAudioSource.Play();
    }
    public void PlayWinSFX()
    {
        sfxAudioSource.clip = winClip;
        sfxAudioSource.Play();
        musicAudioSource.Pause(); 
    }
    public void StopAllSounds()
    {
        if (musicAudioSource.isPlaying)
            musicAudioSource.Stop();
        if (sfxAudioSource.isPlaying)
            sfxAudioSource.Stop();
    }

    public void PlayDieSFX()
    {
        StopAllSounds();
        sfxAudioSource.clip = dieClip;
        sfxAudioSource.Play();
    }
    public void PlaySwordSFX()
    {
        sfxAudioSource.clip = swordClip;
        sfxAudioSource.Play();

    }
}
