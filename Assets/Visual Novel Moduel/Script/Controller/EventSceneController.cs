using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNGameModuel.Controller
{
    public class EventCGController : MonoBehaviour
    {
        [SerializeField]

        private SpriteController spriteController;

        private const float defaultDuration = 0.5f;

        private void Awake()
        {
            spriteController = GetComponent<SpriteController>();
            if (spriteController == null) {
                spriteController = gameObject.AddComponent<SpriteController>();
            }
        }

        public void NextEventScene(Sprite sprite, float transitionDuration = defaultDuration)
        {
            StartCoroutine(spriteController.ChangeSpriteCrossFade(sprite, transitionDuration));
        }

        public void ChangeEventScene(Sprite sprite, float transitionDuration = defaultDuration)
        {
            StartCoroutine(spriteController.ChangeSpriteWithFadeToBlack(sprite, transitionDuration));
        }

        public void DismissBackground()
        {
            StartCoroutine(spriteController.FadeOutSprite());
        }
    }
}