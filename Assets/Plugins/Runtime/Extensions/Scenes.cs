using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
#endif

namespace Runtime
{
    public static partial class Scenes
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