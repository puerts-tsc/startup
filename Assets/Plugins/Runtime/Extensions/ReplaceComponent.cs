using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameEngine.Extensions {

/// <summary>
///     https://xbuba.com/questions/54973000
/// </summary>
public static class ReplaceComponent {

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component =>
        go.AddComponent<T>().GetCopyOf(toAdd);

    public static void Init<T>(this GameObject go, T comp) where T : Component
    {
        go.AddComponent(comp);
    }

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        var type = comp.GetType();

        if (type != other.GetType()) {
            return null; // type mis-match
        }

        const BindingFlags flags = BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Default |
            BindingFlags.DeclaredOnly;

        var pinfos = type.GetProperties(flags);

        foreach (var pinfo in pinfos.Where(pinfo => pinfo.CanWrite)) {
            try {
                pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
            } catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
        }

        var finfos = type.GetFields(flags);

        foreach (var finfo in finfos) {
            finfo.SetValue(comp, finfo.GetValue(other));
        }

        return comp as T;
    }

}

}