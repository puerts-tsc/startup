using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Runtime.Extensions
{
    public static partial class SceneExtensions
    {
        public static void SetDirty(this Scene scene)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                EditorSceneManager.MarkSceneDirty(scene);
            }
#endif
        }
    }
}