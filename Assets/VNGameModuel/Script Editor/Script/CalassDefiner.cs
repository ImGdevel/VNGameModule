using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace DialogueSystem.Editor
{
#if UNITY_EDITOR

    [InitializeOnLoad]
    public class CalassDefiner : UnityEditor.Editor
    {
        public static readonly string[] Symbols = new string[] {
         "DialogueSystem"
     };

        static CalassDefiner()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }
    }

    public static class SubclassFinder
    {
        public static List<Type> GetSubclasses<T>()
        {
            List<Type> subclasses = new List<Type>();
            Type baseType = typeof(T);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(baseType) || type == baseType)
                    {
                        subclasses.Add(type);
                    }
                }
            }

            return subclasses;
        }
    }

#endif
}
