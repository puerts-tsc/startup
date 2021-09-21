using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Puerts;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
#endif

namespace Runtime
{
    [ExecuteAlways]
    public class JsMain : MonoBehaviour
    {
        public class CustomJsLoader : ILoader
        {
            private string root = "";
            public CustomJsLoader() { }

            public CustomJsLoader( string root )
            {
                this.root = root;
            }

            private string PathToUse( string filepath )
            {
                return filepath.EndsWith( ".cjs" ) ? filepath.Substring( 0, filepath.Length - 4 ) : filepath;
            }

            public bool FileExists( string filepath )
            {
                Debug.Log( $"require: {filepath}" );
                if ( Application.isEditor && File.Exists( Path.Combine( root, filepath ) ) ) {
                    return true;
                }
                else {
                    try {
                        var location = Addressables.LoadResourceLocationsAsync( $"Res/{filepath}.txt" )
                            .WaitForCompletion();
                        if ( location.Any() ) {
                            Debug.Log( $"{filepath} addressable exists." );
                            return true;
                        }
                    }
                    catch ( TargetException e ) {
                        Debug.Log( e.Message );
                    }

                    string pathToUse = this.PathToUse( filepath );
                    var asset = UnityEngine.Resources.Load( pathToUse );
                    if ( asset == null ) {
                        Debug.Log( $"{filepath} not exist" );
                    }
                    else {
                        Debug.Log( $"{filepath} resource exists." );
                    }

                    return asset != null;
                }
            }

            public string ReadFile( string filepath, out string debugpath )
            {
                debugpath = Path.GetFullPath( Path.Combine( root, filepath ) );
                if ( Application.isEditor && File.Exists( debugpath ) ) {
                    Debug.Log( debugpath );
                    return File.ReadAllText( debugpath );
                }

                UnityEngine.TextAsset file = null;
                try {
                    var location = Addressables.LoadResourceLocationsAsync( $"Res/{filepath}.txt" ).WaitForCompletion();
                    if ( location.Any() ) {
                        file = Addressables.LoadAssetAsync<TextAsset>( $"Res/{filepath}.txt" ).WaitForCompletion();
                    }
                }
                catch ( TargetException e ) {
                    Debug.Log( e.Message );
                }

                if ( file == null ) {
                    string pathToUse = this.PathToUse( filepath );
                    file = (UnityEngine.TextAsset)UnityEngine.Resources.Load( pathToUse );
                }
#if UNITY_EDITOR
                if ( file != null ) {
                    debugpath = Path.GetFullPath( Path.Combine( Application.dataPath, "..",
                        AssetDatabase.GetAssetPath( file ) ) );
                }
#endif
                return file?.text;
            }
        }

        static JsEnv jsEnv;
        public static JsEnv env => CheckOrLoadEnv();
        static int port = 9222;
        bool isReady => jsEnv != null && jsEnv.isolate != IntPtr.Zero;
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

            CheckOrLoadEnv();
        }

        static TsConfig config => TsConfig.instance ?? TsConfig.FindAsset();

        static JsEnv CheckOrLoadEnv()
        {
            if ( jsEnv == null || jsEnv.isolate == IntPtr.Zero ) {
                var root = Path.GetFullPath( $"{Application.dataPath}/../{config.OutputPath}" );
                Debug.Log( $"root: {root}" );
                var mode = Application.isEditor ? JsEnvMode.Node : JsEnvMode.Default;
                jsEnv = new JsEnv( new CustomJsLoader( root ), port, mode );
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