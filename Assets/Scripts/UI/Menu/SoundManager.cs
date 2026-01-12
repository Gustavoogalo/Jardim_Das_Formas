using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Lists")]
    [SerializeField] private List<SoundEffect> musicList = new List<SoundEffect>();
    [SerializeField] private List<SoundEffect> sfxList = new List<SoundEffect>();

    // Dicionários para busca rápida (O(1)) em vez de percorrer a lista toda vez
    private Dictionary<string, AudioClip> musicDictionary;
    private Dictionary<string, AudioClip> sfxDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionaries();
            LoadVolume();
        }
        else { Destroy(gameObject); }
    }

    private void InitializeDictionaries()
    {
        musicDictionary = musicList.ToDictionary(s => s.name, s => s.clip);
        sfxDictionary = sfxList.ToDictionary(s => s.name, s => s.clip);
    }

    #region Play Methods

    /// <summary>
    /// Toca uma música de fundo. Se já houver uma tocando, ela será substituída.
    /// </summary>
    public void PlayMusic(string name, bool loop = true)
    {
        if (musicDictionary.TryGetValue(name, out AudioClip clip))
        {
            if (musicSource.clip == clip && musicSource.isPlaying) return; // Já está tocando

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Música '{name}' não encontrada no SoundManager!");
        }
    }

    /// <summary>
    /// Toca um efeito sonoro (SFX) pontual.
    /// </summary>
    public void PlaySFX(string name)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' não encontrado no SoundManager!");
        }
    }

    #endregion

    #region Volume Management

    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void ChangeSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
    }

    public float GetMusicVolume() => musicSource.volume;
    public float GetSFXVolume() => sfxSource.volume;

    #endregion
}