using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Puerts;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
#endif

namespace Runtime
{
    [ExecuteAlways]
    public class JsMain : MonoBehaviour
    {
        static JsEnv jsEnv;
        public static JsEnv env => CheckOrLoadEnv();
        bool isReady => jsEnv != null && jsEnv.isolate != IntPtr.Zero;
        public static TsConfig config => TsConfig.instance;
#if UNITY_EDITOR
        [InitializeOnLoadMethod, DidReloadScripts]
#endif
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded )]
        static async void LoadEnv()
        {
            try {
                // fix:第一次调用 Addressables.LoadResourceLocationsAsync 会异常, 第二次以后正常
                Addressables.LoadResourceLocationsAsync( $"Res/QuickStart.js.txt" ).WaitForCompletion();
            }
            catch ( TargetException e ) { }

            if ( !TsConfig.hasInstance ) {
                TsConfig.OnLoad += () => CheckOrLoadEnv();
            }
            else {
                CheckOrLoadEnv();
            }
        }


        static JsEnv CheckOrLoadEnv()
        {
            if ( jsEnv == null || jsEnv.isolate == IntPtr.Zero ) {
                var root = Path.GetFullPath( $"{Application.dataPath}/../{config.outputPath}" );
                Debug.Log( $"root: {root}" );
                var mode = Application.isEditor ? JsEnvMode.Node : JsEnvMode.Default;
                jsEnv = new JsEnv( new CustomJsLoader( root ), config.debugPort, mode );
                AutoUsing();
                jsEnv.Eval( @"require('log-plus')" );
                RunQuickStart();
//                var firstRun = 
//                if ( firstRun == null ) {
//                    EditorPrefs.SetBool( "NodeTSCAndHotReload.justShutdownByReload", false );
//                }
//                else {
//                    firstRun.Invoke();
//                }
            }

            return jsEnv;
        }

        public static Assembly GetAssemblyByName( string name )
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault( assembly => assembly.GetName().Name == name );
        }

        const string k_CSharpAssembly = "Assembly-CSharp";

        public static void AutoUsing()
        {
            var a = GetAssemblyByName( k_CSharpAssembly )?.GetType( "PuertsStaticWrap.AutoStaticCodeUsing" );
            var mi = a?.GetMethod( "AutoUsing", BindingFlags.Static | BindingFlags.Public );
            mi?.Invoke( null, new object[] { jsEnv } );
            if ( mi == null ) Debug.Log( "AutoUsing Missing!" );
        }

        public void Start()
        {
            if ( Application.isPlaying ) {
                DontDestroyOnLoad( gameObject );
            }

            env.Eval( "$log('JsMain Started')" );
        }

        [ButtonGroup( "test" )]
        static void SayHello() => env.Eval( @"$log('hello, puerts')" );

        [ButtonGroup( "test" )]
        static void RunQuickStart() => jsEnv.Eval<Action>( "require('QuickStart').default" )?.Invoke();

        void Update()
        {
            env.Tick();
        }
    }
}