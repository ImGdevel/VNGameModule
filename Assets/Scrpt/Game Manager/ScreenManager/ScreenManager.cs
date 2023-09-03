using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public ScreenManager Instance {get; private set; }

    void Awake() {
        if(Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    // 해상도 변경 버튼을 클릭할 때 호출되는 함수
    public static void ChangeResolution(int width, int height) {
        // 설정할 해상도 생성
        Resolution newResolution = new Resolution {
            width = width,
            height = height,
            refreshRate = Screen.currentResolution.refreshRate // 현재 리프레시 속도 사용
        };

        // 설정한 해상도 적용
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }

    // 해상도 변경 버튼을 클릭할 때 호출되는 함수
    public static void ChangeResolution(Resolution resolution) {
        // 설정할 해상도 생성
        Resolution newResolution = new Resolution {
            width = resolution.width,
            height = resolution.height,
            refreshRate = Screen.currentResolution.refreshRate // 현재 리프레시 속도 사용
        };

        // 설정한 해상도 적용
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }

    // 전체 화면 모드로 변경
    public static void SetFullScreen(bool setMode) {
        Screen.fullScreen = setMode;
    }

    public static void SetFullScreenMode(FullScreenMode screenMode) {
        Screen.fullScreenMode = screenMode;
    }

    // 프레임 속도 제한 설정
    public static void SetFrameRateLimit(int frameRate) {
        // 음수 또는 0인 경우에는 제한이 없음을 의미합니다.
        if (frameRate <= 0) {
            Application.targetFrameRate = -1;
        }
        else {
            Application.targetFrameRate = frameRate;
        }
    }

    // 그래픽 품질 설정
    public static void SetGraphicsQuality(int qualityLevel) {
        // 유효한 품질 수준 (0부터 QualitySettings.names.Length - 1까지)으로 제한
        qualityLevel = Mathf.Clamp(qualityLevel, 0, QualitySettings.names.Length - 1);

        // 그래픽 품질 설정 변경
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    // 안티앨리어싱 설정
    public static void SetAntiAliasingLevel(int antiAliasingLevel) {
        // 유효한 안티앨리어싱 수준으로 제한
        antiAliasingLevel = Mathf.Clamp(antiAliasingLevel, 0, 8); // 최대 레벨을 8로 제한
        QualitySettings.antiAliasing = antiAliasingLevel; // 실제 Unity의 QualitySettings에 값을 설정


        // 안티앨리어싱 설정 변경
        QualitySettings.antiAliasing = antiAliasingLevel;
    }

    // V-Sync 설정
    public static void SetVSync(int vSyncLevel) {
        // 유효한 V-Sync 레벨로 제한
        vSyncLevel = Mathf.Clamp(vSyncLevel, 0, 2);

        // V-Sync 설정 변경
        QualitySettings.vSyncCount = vSyncLevel;
    }
}
