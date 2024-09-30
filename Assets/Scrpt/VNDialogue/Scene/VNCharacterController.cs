using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCharacterController : MonoBehaviour
{
    private Dictionary<string, VNSpriteController> characters;

    private const float defaultDuration = 0.1f;

    private void Awake()
    {
        characters = new Dictionary<string, VNSpriteController>();
    }

    /// <summary>
    /// 캐릭터를 특정 스프라이트로 보여줌 (크로스 페이드)
    /// </summary>
    public void ShowCharacter(string name, Sprite sprite, float transitionDuration = defaultDuration)
    {
        if (!characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            characterSprite = CreateNewCharacter(name);
        }

        StartCoroutine(characterSprite.ChangeSpriteCrossFade(sprite, transitionDuration));
    }

    /// <summary>
    /// 캐릭터 이미지 변경
    /// </summary>
    public void ChangeCharacter(string name, Sprite sprite, float transitionDuration = defaultDuration)
    {
        if (!characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            characterSprite = CreateNewCharacter(name);
        }

        StartCoroutine(characterSprite.ChangeSpriteCrossFade(sprite, transitionDuration));
    }

    /// <summary>
    /// 캐릭터 위치 이동
    /// </summary>
    public void MoveCharacter(string name, Vector2 position, Vector2 scale, float transitionDuration = defaultDuration)
    {
        if (!characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            characterSprite = CreateNewCharacter(name);
        }

        StartCoroutine(characterSprite.MoveAndZoomSprite(position, scale, transitionDuration));
    }

    /// <summary>
    /// 캐릭터 퇴장 (페이드 아웃)
    /// </summary>
    public void DismissCharacter(string name, float transitionDuration = defaultDuration)
    {

        if (!characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            characterSprite = CreateNewCharacter(name);
        }

        StartCoroutine(characterSprite.FadeOutSprite(transitionDuration));
    }

    private VNSpriteController CreateNewCharacter(string name)
    {
        GameObject characterObj = new GameObject("Character(" + name + ")");
        VNSpriteController spriteController = characterObj.AddComponent<VNSpriteController>();
        characterObj.transform.SetParent(transform);
        characterObj.transform.position = new Vector3(0, -2, 0);

        characters.Add(name, spriteController);

        return spriteController;
    }
}
