using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Runtime
{
    [CreateAssetMenu( fileName = "TsConfig", menuName = "tsc/TsConfig", order = 0 )]
    public class TsConfig : ScriptableObject
    {
        static TsConfig m_Instance;

        public static TsConfig instance => m_Instance ?? ( m_Instance = FindAsset() );
        // ?? ( m_Instance = FindAsset() ?? ScriptableObject.CreateInstance<TsConfig>() );

        public static bool hasInstance => m_Instance != null;
        public string outputPath => "Assets/Scripts/dist~";
        public bool isRunQuickStart;

//        [Header( "ts项目目录" ), SerializeField]
//        public Object TsRoot;
        [FormerlySerializedAs( "m_DebugPort" )]
        [FormerlySerializedAs( "m_Port" )]
        [SerializeField]
        [Header( "Inspector 调试接口" )]
        public int debugPort = 9229;

        [FormerlySerializedAs( "TscRoot" )]
        [SerializeField, HideInInspector]
        UnityEngine.Object m_TscRoot;

        [ShowInInspector, PropertyOrder( -1 )]
        UnityEngine.Object TscRootDir {
            get => m_TscRoot;
            set {
                m_TscRoot = value;
            #if UNITY_EDITOR
                rootPath = AssetDatabase.GetAssetPath( m_TscRoot );
                if ( rootPath != null && !Directory.Exists( rootPath ) ) {
                    rootPath = Path.GetDirectoryName( rootPath );
                }
            #endif
            }
        }

        public bool isLogDebugPath {
            get { return m_IsLogDebugPath; }
            set { m_IsLogDebugPath = value; }
        }

        [ReadOnly]
        public string rootPath;

        public bool isWatching;
        public bool isPause;
        public bool isLog;
        [FormerlySerializedAs( "isReportLoaderLog" )]
        public bool isLogRequirePath;

        [Title( "脚本目录", "目录里面必须存在 `src/` 和 `package.json`", horizontalLine: false )]
        [SerializeField]
        List<Object> dirs = new List<Object>();

        [ReadOnly, ShowInInspector]
        public readonly List<string> sourcePath = new List<string>();

        [SerializeField]
        bool m_IsLogDebugPath;

        public static event Action OnLoad;

        void OnEnable()
        {
            if ( instance == null ) {
                m_Instance = this;
                Debug.Log( $"TsConfig Loaded.output path: {this.outputPath}" );
                OnLoad?.Invoke();
            }
            
        }

        void OnDisable()
        {
            if ( instance == this ) {
                m_Instance = null;
            }
        }

        public static TsConfig FindAsset()
        {
            TsConfig asset = null;
        #if UNITY_EDITOR
            asset = AssetDatabase.FindAssets( $"t:{typeof(TsConfig).Name}" ).Select( guid =>
                AssetDatabase.LoadAssetAtPath<TsConfig>( AssetDatabase.GUIDToAssetPath( guid ) ) ).FirstOrDefault();
        #endif
            return asset;
        }

    #if UNITY_EDITOR
        void OnValidate()
        {
            sourcePath.Clear();
            dirs.Select( t => AssetDatabase.GetAssetPath( t ) )
                .Where( t => Directory.Exists( $"{t}/src" ) && File.Exists( $"{t}/package.json" ) )
                .ForEach( path => sourcePath.Add( path ) );
        }

        [MenuItem( "NodeTSC/Config" )]
        static void OpenConfig()
        {
            Debug.Log( $"found : {AssetDatabase.OpenAsset( FindAsset() )}" );
        }

        [MenuItem( "NodeTSC/生成脚本目录" ), Button]
        public static void GenScriptPath()
        {
            instance.sourcePath.ForEach( t => {
                var dir = $"{instance.outputPath}/{t.Substring( "Assets/".Length )}/src";
                Directory.CreateDirectory( dir );
                File.WriteAllText( $"{dir}/.gitkeep", "" );
            } );
            Debug.Log( "finish" );
        }
    #endif
    }
}