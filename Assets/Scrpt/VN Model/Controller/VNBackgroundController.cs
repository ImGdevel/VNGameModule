using System.Collections.Generic;
using UnityEngine;

public class VNBackgroundController : MonoBehaviour
{
    [SerializeField]
    SpriteList backgroundList;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    private List<Sprite> backgrounds;

    private void Start() {
        backgrounds = backgroundList.spriteList;

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
