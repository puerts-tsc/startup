#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace Helpers
{
    public class HotReload : SerializedMonoBehaviour
    {
        [ButtonGroup( "debug" )]
        void UnWatch() => NodeTscAndHotReload.UnWatch();

        [ButtonGroup( "debug" )]
        void Watch() => NodeTscAndHotReload.Watch();
    }
}
#endif