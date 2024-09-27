using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StaticPath : MonoBehaviour
{

    private static string scriptFilePath;

    public static string staticPath {
        get {
            if (string.IsNullOrEmpty(scriptFilePath)) {
                scriptFilePath = GetScriptFilePath();
            }
            var path = Path.Combine(Path.GetDirectoryName(scriptFilePath), "../Resources/Languages.asset");
            return Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "Assets");
        }
    }

    private static string GetScriptFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null)
    {
        return path;
    }


}
