using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    [SerializeField] BackgroundController backgroundController;
    [SerializeField] CharacterController characterController;


    void Start()
    {
        backgroundController = backgroundController.GetComponent<BackgroundController>();
    }

    public void PlaySceneTransitionEffect(EventData eventData) {
        // 장면 페이드 인

        // 장면 전환

        // 장면 페이드 아웃
    }

    public void PlayCharacterTransitionEffect(EventData eventData) {
        


    }
}
