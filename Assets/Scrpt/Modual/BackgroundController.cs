using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovelGame;

namespace VisualNovelGame
{
    public class BackgroundController : MonoBehaviour, IController
    {
        [SerializeField]
        private SpriteList backgroundData;

        private VNSpriteController spriteController;

        private const float defaultDuration = 0.5f;

        private void Awake()
        {
            GameObject spriteCotrollerObj = new GameObject("Event Scene");
            VNSpriteController spriteController = spriteCotrollerObj.AddComponent<VNSpriteController>();
            spriteController.SetSpriteList(backgroundData.spriteList);
            spriteCotrollerObj.transform.SetParent(transform);
        }

        public void SetScenario(Scenario scenario)
        {
            // 배경 관련 데이터 설정

            // 배경 id와 배경 전환(나타나는) 이벤트에 따라서 변환


        }

        public void SetBackground(int index)
        {
            spriteController.ShowSpriteInstant(index);
        }

        public void NextBackground(int index, float transitionDuration = defaultDuration)
        {
            StartCoroutine(spriteController.ChangeSpriteCrossFade(index, transitionDuration));
        }

        public void ChangeBackground(int index, float transitionDuration = defaultDuration)
        {
            StartCoroutine(spriteController.ChangeSpriteWithFadeToBlack(index, transitionDuration));
        }

        public void CloseBackground()
        {
            StartCoroutine(spriteController.FadeOutSprite());
        }

    }


}
