#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Puerts;
using Runtime;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    [InitializeOnLoad]
    public static class NodeTscAndHotReload
    {
        static JsEnv env => JsMain.env;
        public static Action<int> addDebugger;
        public static Action<int> removeDebugger;

        static NodeTscAndHotReload()
        {
            Debug.Log( "NodeTSC start" );
            EditorApplication.update += Update;
            JsEnv.OnJsEnvCreate += OnJsEnvCreate;
            JsEnv.OnJsEnvDispose += OnJsEnvDispose;
            AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;
            if ( EditorPrefs.GetBool( "NodeTSCAndHotReload.justShutdownByReload" ) ) {
                //EditorPrefs.SetBool( "NodeTSCAndHotReload.justShutdownByReload", false );
                Watch();
            }
        }

        static Dictionary<JsEnv, int> envAndPort = new Dictionary<JsEnv, int>();

        static void OnJsEnvCreate( JsEnv env, ILoader loader, int debugPort )
        {
            if ( debugPort != -1 && env != null && addDebugger != null ) {
                UnityEngine.Debug.Log( "OnJsEnvCreate:" + debugPort );
                envAndPort.Add( env, debugPort );
                addDebugger( debugPort );
            }
        }

        static void OnJsEnvDispose( JsEnv env )
        {
            int debugPort = 0;
            if ( env != null && removeDebugger != null && envAndPort.TryGetValue( env, out debugPort ) ) {
                removeDebugger( debugPort );
            }
        }

        static void Update()
        {
            env?.Tick();
        }

        static void BeforeAssemblyReload()
        {
//        if ( env != null ) {
//            EditorPrefs.SetBool( "NodeTSCAndHotReload.justShutdownByReload", true );
//            //UnWatch();
//        }
        }

        [MenuItem( "NodeTSC/__TIPS__", false, 10 )]
        static void readme()
        {
            EditorUtility.DisplayDialog( "tips", @"
你可以使用这个功能来编译 TsProj 目录的 typescript。
并自动将改动热重载至游戏中（需要打开 inspector ）
但请确认你使用的是 Node 版本 puerts
并且已经在项目目录(即 Assets 上级目录)执行`npm i`

You can use this feature to compile typescript in TsProj/ directory.
It will hot reload the new code in the game with inspector opened.
But please confirm that you're using the Node version Puerts 
and run `npm i` in project directory
        ", "ok" );
        }

        /// <summary>
        /// 删除空行和注释, ts-node不支持 tsconfig.json 里面有注释
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HoverTreeClearMark( this string input )
        {
            input = Regex.Replace( input, @"/\*[\s\S]*?\*/", "", RegexOptions.IgnoreCase );
            input = Regex.Replace( input, @"^\s*//[\s\S]*?$", "", RegexOptions.Multiline );
            input = Regex.Replace( input, @"^\s*$\n", "", RegexOptions.Multiline );
            input = Regex.Replace( input, @"^\s*//[\s\S]*", "", RegexOptions.Multiline );
            return input;
        }

        static string tsRoot => Application.dataPath + "/";

        static string tsConfig =>
            File.ReadAllText( $"{tsRoot}tsconfig.json".HoverTreeClearMark().Replace( "\"", "'" ) );

//    moduleRequire('ts-node').register({
//    compilerOptions: {
//            'strict': false,
//            'strictNullChecks': false,
//            'strictPropertyInitialization': false,
//            'target': 'ES6', 
//            'module': 'commonjs',
//            'sourceMap': true,
//            'skipLibCheck': true,
//            'baseUrl: 'src',
//            'outDir': 'dist',
//            'moduleResolution': 'node',
//            'typeRoots': [
//            '../../node_modules/@types'
//                ]
//        },
//        'include': [ 'src/**/*' ]
//    })
//    moduleRequire('./src/hotReload/compile.ts')

        [MenuItem( "NodeTSC/Compile TsProj" )]
        static void Compile()
        {
            Debug.Log( "compiling" );
            var func = JsMain.env.Eval<Action<string>>( "require('compile').default" );
            var extensions = new List<string> { ".ts", ".tsx" };
            TsConfig.instance.sourcePath.ForEach( dir => {
                var files = JsonUtility.ToJson( Directory
                    .GetFiles( Path.GetFullPath( Application.dataPath + $"/../{dir}/src" ), "*.*",
                        SearchOption.AllDirectories ).Where( f => extensions.IndexOf( Path.GetExtension( f ) ) >= 0 )
                    .ToArray() );
                Debug.Log( files );
                func.Invoke( files );
            } );

//            EditorUtility.DisplayProgressBar( "compile ts", "create jsEnv", 0 );
//            JsEnv env = new JsEnv( JsEnvMode.Node );
//            bool result = env.Eval<bool>( @"
//            try {
//                const moduleRequire = require('module').createRequire('" + tsRoot + @"')
//                moduleRequire('ts-node').register(" + tsConfig + @")
//                moduleRequire('./Scripts/Node-tsc/src/compile.ts')
//
//                true;
//            } catch(e) {
//                console.error(e);
//                console.error('Some error triggered. Maybe you should run `npm i` in project directory');
//                false;
//            }
//        " );
//            if ( !result ) {
//                EditorUtility.ClearProgressBar();
//            }
//
//            env.Dispose();
//            env = null;
        }

        [MenuItem( "NodeTSC/Compile TsProj" ,true)]
        static bool CompileValidate()
        {
            return PuertsDLL.IsJSEngineBackendSupported( JsEnvMode.Node );
        }

        [MenuItem( "NodeTSC/Watch tsProj And HotReload/Restart", false, -101 )]
        [MenuItem( "NodeTSC/Watch tsProj And HotReload/on", false, 1 )]
        static void Watch()
        {
            Debug.Log( "tsRoot: " + tsRoot );
            //env = new JsEnv( JsEnvMode.Node );
            env.UsingAction<int>();
            bool result = env.Eval<bool>( @"
            global.CS = require('csharp');
            process.on('uncaughtException', function(e) { console.error('uncaughtException', e) });
            try {
                const moduleRequire = require('module').createRequire('" + tsRoot + @"')
                moduleRequire('ts-node').register(" + tsConfig + @")
                global.HotReloadWatcher = moduleRequire('./Scripts/Node-tsc/src/watch.ts').default

                const jsEnvs = CS.Puerts.JsEnv.jsEnvs
                console.log('jsEnvs.Count:' + jsEnvs.Count);
                for (let i = 0; i < jsEnvs.Count; i++)
                {
                    const item = jsEnvs.get_Item(i);
                    if (item && item.debugPort != -1) {
                        HotReloadWatcher.addDebugger(item.debugPort)
                    }
                }

                CS.NodeTSCAndHotReload.addDebugger = HotReloadWatcher.addDebugger.bind(HotReloadWatcher);
                CS.NodeTSCAndHotReload.removeDebugger = HotReloadWatcher.removeDebugger.bind(HotReloadWatcher);
                
                true;
            } catch(e) {
                console.error(e.stack);
                console.error('Some error triggered. Maybe you should run `npm i` in project directory');
                false;
            }
        " );
            if ( !result ) {
                UnWatch();
            }
            else {
                TsConfig.instance.isWatching = true;
                UnityEngine.Debug.Log( "watching tsproj" );
            }
        }

        [MenuItem( "NodeTSC/Watch tsProj And HotReload/on", true )]
        static bool WatchValidate()
        {
            return PuertsDLL.IsJSEngineBackendSupported( JsEnvMode.Node ) && !TsConfig.instance.isWatching;
        }

        [MenuItem( "NodeTSC/Watch tsProj And HotReload/off", false, 2 )]
        static void UnWatch()
        {
            env.Dispose();
            //env = null;
            addDebugger = null;
            removeDebugger = null;
            TsConfig.instance.isWatching = false;
            UnityEngine.Debug.Log( "stop watching tsproj" );
        }

        [MenuItem( "NodeTSC/Watch tsProj And HotReload/off", true )]
        static bool UnWatchValidate()
        {
            return PuertsDLL.IsJSEngineBackendSupported( JsEnvMode.Node ) && TsConfig.instance.isWatching;
        }
    }
}
#endif