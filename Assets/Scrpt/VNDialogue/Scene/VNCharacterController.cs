using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCharacterController : MonoBehaviour
{
    [SerializeField]
    private SpriteList[] characterData;

    Dictionary<string, VNSpriteController> characters;

    private const float defaultDuration = 0.1f;

    private void Awake() {
        characters = new Dictionary<string, VNSpriteController>();
        foreach (var character in characterData) {
            string characterName = character.listName;
            GameObject characterObj = new GameObject("character("+ characterName + ")");
            VNSpriteController spriteController = characterObj.AddComponent<VNSpriteController>();
            spriteController.SetSpriteList(character.spriteList);
            characterObj.transform.SetParent(transform);

            Debug.Log(characterName);
            Debug.Log(spriteController);

            characters.Add(characterName, spriteController);
        }
    }
     
    public void ShowCharacter(string name, int index, float transitionDuration = defaultDuration) {
        VNSpriteController characterSprite = characters[name].GetComponent<VNSpriteController>();
        StartCoroutine(characterSprite.ChangeSpriteCrossfade(index, transitionDuration));
    }

    public void MoveCharacter(string name, Vector2 posision, float transitionDuration = defaultDuration) {
        VNSpriteController characterSprite = characters[name].GetComponent<VNSpriteController>();
        StartCoroutine(characterSprite.MoveSpritePosision(posision, transitionDuration));
    }

    public void DismissCharacter(string name, float transitionDuration = defaultDuration) {
        VNSpriteController characterSprite = characters[name].GetComponent<VNSpriteController>();
        StartCoroutine(characterSprite.FadeOutSprite(transitionDuration));
    }

}
