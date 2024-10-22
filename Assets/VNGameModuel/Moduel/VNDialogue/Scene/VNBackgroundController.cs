using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNBackgroundController : MonoBehaviour
{
    [SerializeField]
    private SpriteList backgroundData; // 혹시 이 데이터가 필요한 곳이 있으면 유지

    private VNSpriteController spriteController;

    private const float defaultDuration = 0.5f;

    private void Awake()
    {
        GameObject spriteCotrollerObj = new GameObject("Event Scene");
        spriteController = spriteCotrollerObj.AddComponent<VNSpriteController>();
        spriteCotrollerObj.transform.SetParent(transform);
    }

    // 즉시 배경을 설정
    public void SetBackground(Sprite sprite)
    {
        spriteController.ShowSpriteInstant(sprite);
    }

    // 다음 배경으로 부드럽게 전환 (크로스 페이드)
    public void NextBackground(Sprite sprite, float transitionDuration = defaultDuration)
    {
        StartCoroutine(spriteController.ChangeSpriteCrossFade(sprite, transitionDuration));
    }

    // 배경을 페이드 아웃 후 검은 화면에서 변경
    public void ChangeBackground(Sprite sprite, float transitionDuration = defaultDuration)
    {
        StartCoroutine(spriteController.ChangeSpriteWithFadeToBlack(sprite, transitionDuration));
    }

    // 배경을 페이드 아웃으로 닫기
    public void CloseBackground()
    {
        StartCoroutine(spriteController.FadeOutSprite());
    }
}
