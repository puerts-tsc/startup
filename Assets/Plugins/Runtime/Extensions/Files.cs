using System;
using System.IO;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Extensions {

public static partial class Files {

    public static void RenameTo(this FileInfo fileInfo, string newName)
    {
        if (newName.IsNullOrWhitespace() || fileInfo == null) {
            return;
        }

        Debug.Log($"move to: {Path.Combine(fileInfo.Directory.FullName, newName)}");
        fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newName));
    #if UNITY_EDITOR
        AssetDatabase.Refresh();
    #endif
    }

    public static string AssetPathNormal(this string path)
    {
    #if UNITY_EDITOR
        if (Guid.TryParse(path, out var guid)) {
            path = AssetDatabase.GUIDToAssetPath(path);
        }
    #endif
        return path.Replace('\\', '/').Replace(Application.dataPath, "Assets").Trim('/');
    }

    public static string PathCombine(this string path, params string[] names)
    {
        names.ForEach(s => path = Path.Combine(path, s));

        return path.Replace("\\", "/");
    }

    public static void RenameObject(this Object obj, string newName)
    {
        obj?.dataPathRoot().RenamePath(newName);
    }

    public static void RenamePath(this string fullpath, string newName)
    {
        if (fullpath.IsNullOrWhitespace()) {
            return;
        }

        if (!File.Exists(fullpath)) {
            fullpath = fullpath.dataPathRoot();
        }

        if (File.Exists(fullpath)) {
            new FileInfo(fullpath).RenameTo(newName);
        } else {
            Debug.Log($"{fullpath} not exists");
        }
    }

}

}