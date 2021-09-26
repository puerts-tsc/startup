using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Puerts;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
#if UNITY_EDITOR
#endif

namespace Runtime
{
    [ExecuteAlways]
    public class JsMain : MonoBehaviour
    {
        TsConfig local;
        static JsMain m_instance;
        public static JsMain instance => m_instance ?? ( m_instance = FindObjectOfType<JsMain>( true ) );

        
        [ShowInInspector]
        TsConfig m_Config {
            get => TsConfig.instance;
            set => local = value;
        }

        [ShowInInspector]
        bool Watching => config.isWatching;

        [ShowInInspector, ReadOnly]
        public string TscTickTime { get; set; }

        [ShowInInspector, ReadOnly]
        public string TscReloadTime { get; set; }

        [ShowInInspector, ReadOnly]
        public string EnvTickTime { get; set; }

        public static string SetTickTime {
            set {
                if ( instance != null ) {
                    instance.TscTickTime = value;
                }
            }
        }

        public static string SetTscReloadTime {
            set {
                if ( instance != null ) {
                    instance.TscReloadTime = value;
                }
            }
        }

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

        static Action QuickStart;

        static JsEnv CheckOrLoadEnv()
        {
            if ( jsEnv == null || jsEnv.isolate == IntPtr.Zero ) {
                var root = Path.GetFullPath( config.outputPath );
                var mode = Application.isEditor ? JsEnvMode.Node : JsEnvMode.Default;
                jsEnv = new JsEnv( new CustomJsLoader( root ), config.debugPort, mode );
                AutoUsing();
                QuickStart = jsEnv.Eval<Action>( @"require('QuickStart')?.default" );
                if ( config.isRunQuickStart ) QuickStart?.Invoke();
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

        void Awake()
        {
            m_instance ??= this;
        }

        public static T api<T>( T obj = null ) where T : Api<T>
        {
            return Api<T>.instance;
        }

        public static T GetFairyMeshFactory<T>( T obj1 = default, Component obj = null ) where T : IMeshFactory, new()
        {
            return default;
        }

        public void Start()
        {
            if ( Application.isPlaying ) {
                DontDestroyOnLoad( gameObject );
            }


            m_instance ??= this;
            env.Eval<Action<string>>( "require('QuickStart').$log" ).Invoke( "Puerts Start" );
        }

        [ButtonGroup( "test" )]
        static void SayHello() => env.Eval<Action<string>>( "require('QuickStart').$log" ).Invoke( "hello, puerts" );

        [ButtonGroup( "test" )]
        static void RunQuickStart() => jsEnv.Eval<Action>( "require('QuickStart').default" ).Invoke();

        [ButtonGroup( "env" )]
        static void RestartEnv()
        {
            env.Dispose();
            env.Eval( "console.log('restart')" );
        }

        public static string json( object obj ) => JsonUtility.ToJson( obj ).Of( s => Debug.Log( "Json: " + s ) );

        public static string getTsFiles( string dir )
        {
            var extensions = new List<string> { ".ts", ".tsx" };
            string[] files = Directory.GetFiles( dir, "*.*", SearchOption.AllDirectories )
                .Where( f => extensions.IndexOf( Path.GetExtension( f ) ) >= 0 )
                .Select( s => s.Substring( dir.Length + 1 ).Replace( "\\", "/" ) ).ToArray();
            return JsonConvert.SerializeObject( files ).Of( s => Debug.Log( s ) );
        }

        long lastTickTime;

        void Update()
        {
            var tm = DateTimeOffset.Now.ToUnixTimeSeconds();
            if ( tm != lastTickTime ) {
                lastTickTime = tm;
                EnvTickTime = DateTimeOffset.Now.ToString( "h:mm:ss" );
            }

            env.Tick();
        }

    #if UNITY_EDITOR

        [MenuItem( "NodeTSC/Run/npm install" ), ButtonGroup( "npm" )]
        static void NpmInstall() => Shell.Wsl( "npm install" ).Of( log => Debug.Log( log ) );

        [MenuItem( "NodeTSC/Run/npm run build" ), ButtonGroup( "npm" )]
        static void NpmRunBuild() => Shell.Wsl( "npm run build" ).Of( log => Debug.Log( log ) );

    #endif
    }

    public interface IMeshFactory
    {
        string name { get; set; }
    }
}