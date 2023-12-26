using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public ScreenManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }


    public static void ChangeResolution(int width, int height)
    {
        Resolution newResolution = new Resolution {
            width = width,
            height = height,
            refreshRate = Screen.currentResolution.refreshRate
        };

        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }

    public static void ChangeResolution(Resolution resolution)
    {
        Resolution newResolution = new Resolution {
            width = resolution.width,
            height = resolution.height,
            refreshRate = Screen.currentResolution.refreshRate
        };

        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }

    public static void SetFullScreen(bool setMode)
    {
        Screen.fullScreen = setMode;
    }

    public static void SetFullScreenMode(FullScreenMode screenMode)
    {
        Screen.fullScreenMode = screenMode;
    }

    public static void SetFrameRateLimit(int frameRate)
    {
        if (frameRate <= 0) {
            Application.targetFrameRate = -1;
        }
        else {
            Application.targetFrameRate = frameRate;
        }
    }

    public static void SetGraphicsQuality(int qualityLevel)
    {
        qualityLevel = Mathf.Clamp(qualityLevel, 0, QualitySettings.names.Length - 1);

        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public static void SetAntiAliasingLevel(int antiAliasingLevel)
    {
        antiAliasingLevel = Mathf.Clamp(antiAliasingLevel, 0, 8);
        QualitySettings.antiAliasing = antiAliasingLevel;

        QualitySettings.antiAliasing = antiAliasingLevel;
    }

    public static void SetVSync(int vSyncLevel)
    {
        vSyncLevel = Mathf.Clamp(vSyncLevel, 0, 2);
        QualitySettings.vSyncCount = vSyncLevel;
    }

    IEnumerator ScreenShot()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        texture.Apply();
    }


}
