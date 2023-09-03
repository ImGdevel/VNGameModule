using UnityEngine;
using System.IO;
using TMPro;


public class FolderReader : MonoBehaviour
{
    public TMP_Text text;

    void Start() {

#if !UNITY_EDITOR
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Language");
        string[] subFolders = Directory.GetDirectories(folderPath);

        foreach (string subFolder in subFolders) {
            string folderName = Path.GetFileName(subFolder);
            text.text = folderName;
        }
#endif
    }
}