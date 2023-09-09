using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    private AudioSource audioSource;
    public MusicList musicList;

    private string currentPlayingMusicName;
    private float masterVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float currentVolume;

    const float fadeDuration = 2.0f;

    private Coroutine fadeOutCoroutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        SettingsManager.OnSettingsChanged += SetSoundSetting;
        currentVolume = 0;
    }

    private void OnDestroy() {
        SettingsManager.OnSettingsChanged -= SetSoundSetting;
    }

    public void PlayMusic(string musicName) {
        if (musicName == currentPlayingMusicName) {
            return;
        }

        MusicList.Music selectedMusic = musicList.musicClips.Find(music => music.name == musicName);

        if (selectedMusic != null) {
            if (fadeOutCoroutine != null) {
                StopCoroutine(fadeOutCoroutine);
            }

            if (currentPlayingMusicName != null) {
                CrossfadeToNewMusic(selectedMusic);
            }
            else {
                StartNewMusic(selectedMusic);
            }

            currentPlayingMusicName = musicName;
            currentVolume = selectedMusic.volume;
        }
        else {
            Debug.LogWarning("음악을 찾을 수 없습니다: " + musicName);
        }
    }

    public void PauseMusic() {
        audioSource.Pause();
    }

    public void ResumeMusic() {
        audioSource.UnPause();
    }

    public void StopMusic() {
        currentPlayingMusicName = "";
        StartCoroutine(FadeOutMusic());
    }

    public void SetVolume(float volume) {
        float finalVolume = masterVolume * musicVolume * volume;
        audioSource.volume = Mathf.Clamp01(finalVolume);
    }

    private void SetSoundSetting(Settings settings) {
        masterVolume = settings.soundSettings.masterVolume;
        musicVolume = settings.soundSettings.musicVolume;

        if (!string.IsNullOrEmpty(currentPlayingMusicName)) {
            float finalVolume = masterVolume * musicVolume * currentVolume;
            audioSource.volume = Mathf.Clamp01(finalVolume);
        }
    }

    private void CrossfadeToNewMusic(MusicList.Music newMusic) {
        StartCoroutine(FadeOutMusic(() => {
            StartCoroutine(FadeInNewMusic(newMusic));
        }));
    }

    private void StartNewMusic(MusicList.Music newMusic) {
        audioSource.volume = 0.0f;
        audioSource.clip = newMusic.audio;
        audioSource.Play();
        StartCoroutine(FadeInMusic(musicVolume * masterVolume));
    }

    private IEnumerator FadeInMusic(float targetVolume) {
        float startVolume = audioSource.volume;

        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration) {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeInNewMusic(MusicList.Music newMusic) {
        audioSource.clip = newMusic.audio;
        audioSource.Play();

        float targetVolume = musicVolume * masterVolume;

        float startVolume = 0.0f;

        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration) {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutMusic() {
        float startVolume = audioSource.volume;

        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration) {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0.0f, t);
            yield return null;
        }

        audioSource.volume = 0.0f;
        audioSource.Stop();
    }

    private IEnumerator FadeOutMusic(Action onFadeOutComplete) {
        float startVolume = audioSource.volume;

        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration) {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0.0f, t);
            yield return null;
        }

        audioSource.volume = 0.0f;
        audioSource.Stop();

        onFadeOutComplete?.Invoke();
    }
}
