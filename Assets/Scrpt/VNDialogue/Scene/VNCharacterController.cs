using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNCharacterController : MonoBehaviour
{
    [SerializeField]
    private SpriteList[] characterData;

    private Dictionary<string, VNSpriteController> characters;

    private const float defaultDuration = 0.1f;

    private void Awake() {
        characters = new Dictionary<string, VNSpriteController>();
        foreach (var character in characterData) {
            string characterName = character.listName;
            GameObject characterObj = new GameObject("Character(" + characterName + ")");
            VNSpriteController spriteController = characterObj.AddComponent<VNSpriteController>();
            spriteController.SetSpriteList(character.spriteList);
            characterObj.transform.SetParent(transform);
            characterObj.transform.position = new Vector3(0,-2,0);

            characters.Add(characterName, spriteController);
        }
    }

    public void ShowCharacter(string name, int index, float transitionDuration = defaultDuration) {
        if (characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            StartCoroutine(characterSprite.ChangeSpriteCrossFade(index, transitionDuration));
        }
        else {
            Debug.LogError("Character not found: " + name);
        }
    }

    public void MoveCharacter(string name, Vector3 position, float transitionDuration = defaultDuration) {
        if (characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            StartCoroutine(characterSprite.MoveSpritePosition(position, transitionDuration));
        }
        else {
            Debug.LogError("Character not found: " + name);
        }
    }

    public void DismissCharacter(string name, float transitionDuration = defaultDuration) {
        if (characters.TryGetValue(name, out VNSpriteController characterSprite)) {
            StartCoroutine(characterSprite.FadeOutSprite(transitionDuration));
        }
        else {
            Debug.LogError("Character not found: " + name);
        }
    }
}