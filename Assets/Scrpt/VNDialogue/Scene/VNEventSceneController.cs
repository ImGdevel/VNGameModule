using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNEventSceneController : MonoBehaviour
{
    [SerializeField]
    private SpriteList eventSceneData;

    private VNSpriteController spriteController;

    private const float defaultDuration = 0.5f;

    private void Awake() {
        GameObject spriteCotrollerObj = new GameObject("Event Scene");
        VNSpriteController spriteController = spriteCotrollerObj.AddComponent<VNSpriteController>();
        spriteController.SetSpriteList(eventSceneData.spriteList);
        spriteCotrollerObj.transform.SetParent(transform);
    }

    public void NextEventScene(int index, float transitionDuration = defaultDuration) {
        StartCoroutine(spriteController.ChangeSpriteCrossFade(index,transitionDuration));
    }

    public void ChangeEventScene(int index, float transitionDuration = defaultDuration) {
        StartCoroutine(spriteController.ChangeSpriteWithFadeToBlack(index,transitionDuration));
    }

    public void CloseEventScene() {
        StartCoroutine(spriteController.FadeOutSprite());
    }
}
