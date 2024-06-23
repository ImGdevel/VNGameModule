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
        private bool isActiveBackup;
        private bool isActiveEncryption;

        /// <summary>
        /// Data Manager Init
        /// </summary>
        /// <param name="fileName">save file name</param>
        /// <param name="isActiveBackup">save backup</param>
        public DataManager(string fileName, bool isActiveEncryption = false, bool isActiveBackup = false)
        {
            filePath = Application.persistentDataPath + "/" + fileName + ".json";
            this.isActiveBackup = isActiveBackup;
        }

        public void SaveData(T data)
        {
            if (isActiveBackup && File.Exists(filePath)) {
                string backupFilePath = filePath + ".backup";
                File.Copy(filePath, backupFilePath, true);
            }

            string json = data.ToJson();
            string file = (isActiveEncryption) ? Encryption.EncryptAES(json) : json;
            File.WriteAllText(filePath, file);
        }

        public T LoadData()
        {
            if (File.Exists(filePath)) {
                string file = File.ReadAllText(filePath);
                string json = (isActiveEncryption) ? Encryption.EncryptAES(file) : file;

                T data = new T();
                data.FromJson(json);
                return data;
            }
            return new T();
        }
    }
}


