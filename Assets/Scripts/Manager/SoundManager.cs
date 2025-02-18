using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField][Range(0f, 1f)] float soundEffectVolume;
    [SerializeField][Range(0f, 1f)] float soundEffectPitchVariance;
    [SerializeField][Range(0f, 1f)] float musicVolume;

    AudioSource musicAudioSource;
    public AudioClip musicClip;

    public SoundSource soundSourcePrefab;

    private void Awake()
    {
        Instance = this;
        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.volume = musicVolume;
        musicAudioSource.loop = true;
    }

    private void Start()
    {
        ChangeBackgoundMusic(musicClip);
    }

    public void ChangeBackgoundMusic(AudioClip clip)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public static void PlayClip(AudioClip clip)
    {
        SoundSource obj = Instantiate(Instance.soundSourcePrefab);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, Instance.soundEffectVolume, Instance.soundEffectPitchVariance);
    }
}
