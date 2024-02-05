using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VNSceneEditor : MonoBehaviour
{
    public string chapter = "chapter01";

    [ArrayElementTitle("sceneId")]
    public List<VNScene> sceneList;

    void Start()
    {
        
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
