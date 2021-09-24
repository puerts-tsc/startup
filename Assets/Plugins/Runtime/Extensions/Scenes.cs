#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;



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