using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNEventSceneController : MonoBehaviour
{
    [SerializeField]
    private SpriteList eventSceneData;

    private VNSpriteController spriteController;

    private const float defaultDuration = 0.1f;

    private void Awake() {
        GameObject spriteCotrollerObj = new GameObject("Event Scene");
        VNSpriteController spriteController = spriteCotrollerObj.AddComponent<VNSpriteController>();
        spriteController.SetSpriteList(eventSceneData.spriteList);
        spriteCotrollerObj.transform.SetParent(transform);

    }

    public void ShowEventScene(int name, float transitionDuration = defaultDuration) {



        
    }

    public void ChangeEventScene(int index, float transitionDuration = defaultDuration) {

    }
}
