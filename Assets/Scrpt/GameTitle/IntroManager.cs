using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public float startFadeInDuration = 2.0f;
    public float introLogoDuration = 4.0f;
    public float endFadeOutDuration = 2.0f;
    public string nextSceneName = "GameTitle";

    private float timer = 0.0f;

    void Update() {
        timer += Time.deltaTime;

        if (timer >= introLogoDuration + startFadeInDuration) {
            VNScreenController.Instance.FadeOut(endFadeOutDuration);
        }

        if (timer >= introLogoDuration + startFadeInDuration + endFadeOutDuration) {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
