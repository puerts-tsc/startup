﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;

namespace ParadoxNotion.Design
{
    /// AssetDatabase related utility
    partial class EditorUtils
    {
        ///Create asset of type T with a dialog prompt to chose path
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            return (T) CreateAsset(typeof(T));
        }

        ///Create asset of type T at target path
        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            return (T) CreateAsset(typeof(T), path);
        }

        ///Create asset of type and show or not the File Panel
        public static ScriptableObject CreateAsset(System.Type type)
        {
            ScriptableObject asset = null;
            var path = EditorUtility.SaveFilePanelInProject("Create Asset of type " + type.ToString(),
                type.Name + ".asset", "asset", "");
            asset = CreateAsset(type, path);
            return asset;
        }

        ///Create asset of type at target path
        public static ScriptableObject CreateAsset(System.Type type, string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }

            ScriptableObject data = null;
            data = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            return data;
        }

        ///----------------------------------------------------------------------------------------------
        ///Get a unique path at current project selection for creating an asset, providing the "filename.type"
        public static string GetAssetUniquePath(string fileName)
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            }

            if (System.IO.Path.GetExtension(path) != "") {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            return AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName);
        }

        ///Get MonoScript reference from type if able
        public static MonoScript MonoScriptFromType(System.Type targetType)
        {
            if (targetType == null) return null;
            var typeName = targetType.Name;
            if (targetType.IsGenericType) {
                targetType = targetType.GetGenericTypeDefinition();
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
            }

            return AssetDatabase.FindAssets(string.Format("{0} t:MonoScript", typeName))
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .FirstOrDefault(m => m != null && m.GetClass() == targetType);
        }

        [MenuItem("Debug/Find ts files")]
        static void FindTsFileTest()
        {
            AssetDatabase.FindAssets($"t:TextAsset")
                .Select(AssetDatabase.GUIDToAssetPath).Where(t => t.Contains(".ts")).ForEach(t => { Debug.Log(t); });
            Debug.Log("end");
        }

        public static TextAsset  GenTsFile(System.Type type)
        {
            var path = $"../Packages/src/app/actions/T{type.Name}.ts";
            var root = type.FullName!.Split('.').FirstOrDefault();
            var content = $@"
import {{ {root}, UnityEngine }} from 'csharp';
import {type.Name} = {type.FullName};
import Debug = UnityEngine.Debug;

export class T{type.Name} extends {type.Name} {{
        public OnExecute() {{

        }}
}}
";
            var text = new TextAsset(content);
            AssetDatabase.CreateAsset(text,path);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        }

        ///Opens the MonoScript of a type if able
        public static bool OpenTypeScriptOfType(System.Type type)
        {
            var mono = MonoScriptFromType(type);
            if (mono != null) {
                var asset = AssetDatabase.FindAssets($"T{type.Name} t:TextAsset").Select(AssetDatabase.GUIDToAssetPath)
                    //.Where(t => t.Contains($"T{type.Name}.ts"))
                    .Select(AssetDatabase.LoadAssetAtPath<TextAsset>)
                    .FirstOrDefault(m => m != null);
                if (asset != null) {
                    Debug.Log(AssetDatabase.GetAssetPath(asset));
                    AssetDatabase.OpenAsset(asset);
                }
                else {
                    Debug.Log($"T{type.Name} not found");
                    AssetDatabase.OpenAsset(GenTsFile(type));
                }

                return true;
            }

            ParadoxNotion.Services.Logger.Log(
                $"Can't open script of type '{type.FriendlyName()}', because a script with the same name does not exist.");
            return false;
        }

        ///Opens the MonoScript of a type if able
        public static bool OpenScriptOfType(System.Type type)
        {
            var mono = MonoScriptFromType(type);
            if (mono != null) {
                AssetDatabase.OpenAsset(mono);
                return true;
            }

            ParadoxNotion.Services.Logger.Log(string.Format(
                "Can't open script of type '{0}', because a script with the same name does not exist.",
                type.FriendlyName()));
            return false;
        }

        ///Asset path to absolute system path
        public static string AssetToSystemPath(UnityEngine.Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            return AssetToSystemPath(assetPath);
        }

        ///Asset path to absolute system path
        public static string AssetToSystemPath(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath)) {
                return Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')) + '/' + assetPath;
            }

            return null;
        }

        ///Get all scene names (added in build settings)
        public static List<string> GetSceneNames()
        {
            var allSceneNames = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes) {
                if (scene.enabled) {
                    var name = scene.path.Substring(scene.path.LastIndexOf("/") + 1);
                    name = name.Substring(0, name.Length - 6);
                    allSceneNames.Add(name);
                }
            }

            return allSceneNames;
        }
    }
}

#endif