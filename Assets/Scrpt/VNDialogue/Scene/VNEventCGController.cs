using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNEventCGController : MonoBehaviour
{
    private VNSpriteController spriteController;

    private const float defaultDuration = 0.5f;

    private void Awake()
    {
        GameObject spriteCotrollerObj = new GameObject("Event Scene");
        spriteController = spriteCotrollerObj.AddComponent<VNSpriteController>();
        spriteCotrollerObj.transform.SetParent(transform);
    }

    // 다음 이벤트 장면으로 크로스 페이드 전환
    public void NextEventScene(Sprite sprite, float transitionDuration = defaultDuration)
    {
        StartCoroutine(spriteController.ChangeSpriteCrossFade(sprite, transitionDuration));
    }

    // 이벤트 장면을 검은색으로 페이드 후 전환
    public void ChangeEventScene(Sprite sprite, float transitionDuration = defaultDuration)
    {
        StartCoroutine(spriteController.ChangeSpriteWithFadeToBlack(sprite, transitionDuration));
    }

    // 이벤트 장면을 닫음 (페이드 아웃)
    public void CloseEventScene()
    {
        StartCoroutine(spriteController.FadeOutSprite());
    }
}
