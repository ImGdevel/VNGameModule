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
            // ��� ���� ������ ����

            // ��� id�� ��� ��ȯ(��Ÿ����) �̺�Ʈ�� ���� ��ȯ


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
