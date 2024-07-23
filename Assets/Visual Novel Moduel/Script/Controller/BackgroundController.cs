using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNGameModuel.Controller
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<string, SpriteController> backgrounds = new Dictionary<string, SpriteController>();

        private SpriteController spriteController;

        private const float defaultDuration = 0.5f;

        void Awake()
        {
            spriteController = GetComponent<SpriteController>();
            if (spriteController == null) {
                spriteController = gameObject.AddComponent<SpriteController>();
            }
        }

        public void SetBackground(Sprite sprite)
        {
            spriteController.SetSprite(sprite);
        }

        public void ChangeBackground(Sprite sprite, float transitionDuration = defaultDuration)
        {
            StartCoroutine(spriteController.ChangeSpriteWithFadeToBlack(sprite, transitionDuration));
        }

        public void DismissBackground()
        {
            StartCoroutine(spriteController.FadeOutSprite());
        }

        public void StopEvent()
        {
            spriteController.EndSpriteEffect();
        }

    }
}