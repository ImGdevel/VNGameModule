using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNSceneController : MonoBehaviour
{
    public static VNSceneController Instance { get; private set; }

    [SerializeField] VNBackgroundController backgroundController;
    [SerializeField] VNCharacterController characterController;


    public void Awake() {
        if(Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        backgroundController = backgroundController.GetComponent<VNBackgroundController>();
    }

    public void PlayEventScene(EventData eventData) {
        Data data = eventData.data;
        switch (eventData.type) {
            case "ShowCharacter":
                

                break;
            case "MoveCharacter":


                break;
            case "DismissCharacter":


                break;
            case "BackgroundChange":


                break;
            default:
                break;
        }
    }

    
}
