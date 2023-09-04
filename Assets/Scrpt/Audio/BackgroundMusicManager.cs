using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    private AudioSource audioSource;
    public MusicList musicList;

    private string currentPlayingMusicName; // 현재 재생 중인 음악 이름
    [Range(0f, 1f)] private float masterVolume = 1.0f;
    [Range(0f, 1f)] private float musicVolume = 1.0f;
    [Range(0f, 1f)] public float currentVolume;

    public float fadeDuration = 2.0f;

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
            // 이미 해당 음악을 재생 중이면 아무 작업도 하지 않음
            return;
        }

        MusicList.Music selectedMusic = musicList.musicClips.Find(music => music.name == musicName);

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
                Debug.Log("음악 첫 시행");
                audioSource.volume = 0.0f;
                audioSource.clip = selectedMusic.audio;
                audioSource.Play();
                StartCoroutine(FadeInMusic(currentVolume)); // 볼륨 서서히 증가
            }

            currentPlayingMusicName = musicName;

            // 현재 볼륨을 계산하여 설정
            currentVolume = selectedMusic.volume;
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
}
