using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeetAndTalk.GlobalValue
{
    public class GlobalValueInUI : MonoBehaviour
    {
        [Header("Text Element")]
        public TMPro.TMP_Text Text;
        [Header("Value Settings")]
        public GlobalValueType valueType;
        public string valueName;
        [Header("Text Settings")]
        public string Prefix;
        public string Suffix;

        GlobalValueManager manager;

        private void Awake()
        {
            manager = Resources.Load<GlobalValueManager>("GlobalValue");
            manager.LoadFile();
        }

        private void Update()
        {
            Text.text = $"{Prefix} {GetValue()} {Suffix}";
        }

        public string GetValue()
        {
            if(valueType == GlobalValueType.Int) return manager.Get<int>(valueType, valueName).ToString();
            else if(valueType == GlobalValueType.Float) return manager.Get<float>(valueType, valueName).ToString();
            else if(valueType == GlobalValueType.Bool) return manager.Get<bool>(valueType, valueName).ToString();
            else return manager.Get<string>(valueType, valueName);
        }
    }
}
