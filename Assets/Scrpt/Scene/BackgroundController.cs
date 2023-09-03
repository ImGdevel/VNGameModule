using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    BackgroundList backgroundList;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    private List<Sprite> backgrounds;

    private void Start() {
        backgrounds = backgroundList.backgroundSprites;

        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetBackground(int index) {
        if (index >= 0 && index < backgrounds.Count) {
            spriteRenderer.sprite = backgrounds[index];
        }
        else {
            Debug.LogWarning("Invalid background index: " + index);
        }
    }
}
