using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace UnityTetris.Sandbox
{
    [GlobalConfig("Assets/Config/Tetris")]
    public class SandboxTest : GlobalConfig<SandboxTest>
    {
        [MenuItem("Tetris/Test Cube")]
        static void TestCube() => Instance.Cube();

        void Cube()
        {
            var cur = Selection.activeGameObject;
            Debug.Log(cur?.name, cur);
        }
    }
}