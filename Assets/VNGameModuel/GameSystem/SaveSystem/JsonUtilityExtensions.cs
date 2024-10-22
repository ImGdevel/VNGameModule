using UnityEngine;

namespace Utils
{
    public static class JsonUtilityExtensions
    {
        public static string ToJson<T>(this T data)
        {
            return JsonUtility.ToJson(data, true);
        }

        public static void FromJson<T>(this T data, string json)
        {
            JsonUtility.FromJsonOverwrite(json, data);
        }
    }
}

