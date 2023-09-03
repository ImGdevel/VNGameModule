using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    private static BackgroundMusicManager instance;

    private AudioSource audioSource;
    public MusicList musicList;

    private float masterVolume = 1.0f;
    private float musicVolume = 1.0f;
    private float currentVolume = 1.0f;

    private string currentPlayingMusicName; // 현재 재생 중인 음악 이름

    private Coroutine fadeOutCoroutine;

    private void Awake() {
        if (instance == null) {
            instance = this;
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
    }

    private void OnDestroy() {
        SettingsManager.OnSettingsChanged -= SetSoundSetting;
    }

    public void PlayMusic(string musicName) {
        if (musicName == currentPlayingMusicName) {
            // 이미 해당 음악을 재생 중이면 아무 작업도 하지 않음
            return;
        }

        MusicList.Music selectedMusic = musicList.musicClips.Find(music => music.musicName == musicName);

        if (selectedMusic != null) {
            if (fadeOutCoroutine != null) {
                StopCoroutine(fadeOutCoroutine);
            }

            if (currentPlayingMusicName != null) {
                // 이전 음악을 서서히 페이드 아웃
                fadeOutCoroutine = StartCoroutine(FadeOutMusic());
            }
            else {
                // 이전 음악이 없는 경우, 즉시 음악 재생
                audioSource.volume = 0.0f;
                audioSource.clip = selectedMusic.musicClip;
                audioSource.Play();
            }

            currentPlayingMusicName = musicName;

            // 현재 볼륨을 계산하여 설정
            float finalVolume = masterVolume * musicVolume * selectedMusic.volume;
            currentVolume = finalVolume;
        }
        else {
            // 음악을 찾을 수 없는 경우
            Debug.LogWarning("음악을 찾을 수 없습니다: " + musicName);
        }
    }

    public void PauseMusic() {
        audioSource.Pause();
    }

    public void ResumeMusic() {
        audioSource.UnPause();
    }

    public void SetVolume(float volume) {
        float finalVolume = masterVolume * musicVolume * volume;
        audioSource.volume = Mathf.Clamp01(finalVolume);
    }

    private void SetSoundSetting(Settings settings) {
        masterVolume = settings.soundSettings.masterVolume;
        musicVolume = settings.soundSettings.musicVolume;

        // 설정이 변경될 때 현재 재생 중인 음악에 새로운 볼륨을 적용
        if (!string.IsNullOrEmpty(currentPlayingMusicName)) {
            float finalVolume = masterVolume * musicVolume * currentVolume;
            audioSource.volume = Mathf.Clamp01(finalVolume);
        }
    }

    private IEnumerator FadeOutMusic() {
        float startVolume = audioSource.volume;
        float fadeDuration = 1.0f; // 페이드 아웃에 걸리는 시간

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
}
