using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VNGameModuel.Controller
{
    public class CharacterCGController : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<string, SpriteController> characters = new Dictionary<string, SpriteController>();

        [SerializeField]
        private const float defaultDuration = 0.1f;

        public Vector2 CenterPosition = new Vector2(0, 0);
        public Vector2 LeftPosition = new Vector2(-2, 0);
        public Vector2 RightPosition = new Vector2(2, 0);

        public void SetCharacterByPosition(string characterName, Sprite sprite, CharacterPosition positionType, float transitionDuration = defaultDuration)
        {
            if (!characters.ContainsKey(characterName)) {
                AddCharacter(characterName);
            }
            characters[characterName].SetSpritePosision(GetPosisionByType(positionType, characters[characterName].transform));
            StartCoroutine(characters[characterName].ChangeSpriteCrossFade(sprite, transitionDuration));
        }

        public void ShowCharacter(string characterName, Sprite sprite, float transitionDuration = defaultDuration)
        {
            if (!characters.ContainsKey(characterName)) {
                AddCharacter(characterName);
            }
            StartCoroutine(characters[characterName].ChangeSpriteCrossFade(sprite, transitionDuration));
        }

        public void MoveCharacter(string characterName, Vector3 position, float transitionDuration = defaultDuration)
        {
            if (!characters.ContainsKey(characterName)) {
                AddCharacter(characterName);
            }
            StartCoroutine(characters[characterName].MoveSpritePosition(position, transitionDuration));
        }

        public void DismissCharacter(string characterName, float transitionDuration = defaultDuration)
        {
            if (!characters.ContainsKey(characterName)) {
                AddCharacter(characterName);
            }
            StartCoroutine(characters[characterName].FadeOutSprite(transitionDuration));
        }

        public void StopEvent()
        {
            foreach (SpriteController controller in characters.Values) {
                controller.EndSpriteEffect();
            }
        }

        private void AddCharacter(string characterName)
        {
            GameObject characterObj = new GameObject("Character(" + characterName + ")");
            SpriteController spriteController = characterObj.AddComponent<SpriteController>();
            characterObj.transform.SetParent(transform);
            characterObj.transform.position = new Vector3(0, 0, 0);
            characters.Add(characterName, spriteController);
        }

        private Vector2 GetPosisionByType(CharacterPosition positionType, Transform transform)
        {
            Vector2 position;
            if (positionType == CharacterPosition.Center) {
                position = CenterPosition;
            }
            else if (positionType == CharacterPosition.Left) {
                position = LeftPosition;
            }
            else if (positionType == CharacterPosition.Right) {
                position = RightPosition;
            }
            else {
                position = transform.position;
            }
            return position;
        }
    }
}
