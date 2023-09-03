using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CSVFileLoader
{
    public static List<string[]> DialogueFileLoad(string language) {
        string filePath;
#if UNITY_EDITOR
        filePath = Path.Combine(Application.dataPath, "Language", language, "dialogue.csv");
#else
        filePath = Path.Combine(Application.streamingAssetsPath, ".." , "Language", language ,"dialogue.csv");
#if UNITY_ANDROID
        filePath = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
#endif
#endif
        return FileLoad(filePath);
    }

    public static List<string[]> FileLoad(string filePath) {
        List<string[]> csvData = new List<string[]>();
        try {
            using (StreamReader reader = new StreamReader(filePath)) {
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    csvData.Add(values);
                }
            }
            foreach (string[] values in csvData) {
                foreach (var item in values) {
                    Debug.Log(item);
                }
            }
        }
        catch (IOException e) {
            Debug.LogError("Error reading CSV file: " + e.Message);
        }
        return csvData;
    }
}

