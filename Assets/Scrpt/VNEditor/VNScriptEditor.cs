using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VNSceneEditor : MonoBehaviour
{
    public static VNSceneEditor Instance { private set; get; }

    public string chapter = "chapter01";

    [ArrayElementTitle("sceneId")]
    public List<VNScene> sceneList;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(Instance);
        }
    }


    public void AddNewScene()
    {
        sceneList.Add(new VNScene(chapter, sceneList.Count + 1));
    }

    public void ClearScene()
    {
        sceneList.Clear();
    }

}
