using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utils;

namespace SaveSystem
{

    /// <summary>
    /// Data Manager Calss
    /// Can Easy Save and Load
    /// </summary>
    /// <typeparam name="T"> data type </typeparam>
    public class DataManager<T> where T : new()
    {
        private string filePath;

        public DataManager(string fileName)
        {
            filePath = Application.persistentDataPath + "/" + fileName + ".json";
        }

        public void SaveData(T data)
        {
            string json = data.ToJson();
            File.WriteAllText(filePath, json);
        }

        public T LoadData()
        {
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                T data = new T();
                data.FromJson(json);
                return data;
            }
            return new T();
        }
    }
}


