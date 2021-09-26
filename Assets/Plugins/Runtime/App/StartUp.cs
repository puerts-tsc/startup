using UnityEngine;

namespace Runtime.App
{
    public class StartUp
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void AppSettings()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 2;
            Debug.Log($"GameStart: {Application.version}");
        }
    }
}