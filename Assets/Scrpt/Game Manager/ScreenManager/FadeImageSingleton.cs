using UnityEngine;
using UnityEngine.UI;

public class FadeImageSingleton : MonoBehaviour
{
    public static FadeImageSingleton Instance { get; private set; }
    public Image FadeImage { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            FadeImage = GetComponent<Image>();
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
}
