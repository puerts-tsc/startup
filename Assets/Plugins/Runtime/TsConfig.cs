using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Admin;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Runtime
{
    [CreateAssetMenu( fileName = "TsConfig", menuName = "Node-tsc/TsConfig", order = 0 )]
    public class TsConfig : ScriptableObject
    {
        static TsConfig m_Instance;

        public static TsConfig instance => m_Instance ?? ( m_Instance = FindAsset() );
        // ?? ( m_Instance = FindAsset() ?? ScriptableObject.CreateInstance<TsConfig>() );

        public static bool hasInstance => m_Instance != null;
        public string outputPath => Application.dataPath + "/Gen/dist~";

//        [Header( "ts项目目录" ), SerializeField]
//        public Object TsRoot;
        [FormerlySerializedAs( "m_DebugPort" )]
        [FormerlySerializedAs( "m_Port" )]
        [SerializeField]
        [Header( "Inspector 调试接口" )]
        public int debugPort = 9229;



        public bool isWatching;
        public bool isPause;
        public bool isLog;
        public bool isReportLoaderLog;

        [Title( "脚本目录", "目录里面必须存在 `src/` 和 `package.json`", horizontalLine: false )]
        [SerializeField]
        List<Object> dirs = new List<Object>();

        [ReadOnly, ShowInInspector]
        public readonly List<string> sourcePath = new List<string>();

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

#if UNITY_EDITOR
        void OnValidate()
        {
            sourcePath.Clear();
            dirs.Select( t => AssetDatabase.GetAssetPath( t ) )
                .Where( t => Directory.Exists( $"{t}/src" ) && File.Exists( $"{t}/package.json" ) )
                .ForEach( path => sourcePath.Add( path ) );
        }
#endif

        public static TsConfig FindAsset()
        {
            TsConfig asset = null;
#if UNITY_EDITOR
            asset = AssetDatabase.FindAssets( $"t:{typeof(TsConfig).Name}" ).Select( guid =>
                AssetDatabase.LoadAssetAtPath<TsConfig>( AssetDatabase.GUIDToAssetPath( guid ) ) ).FirstOrDefault();
#endif
            if ( asset != null ) {
                //Debug.Log( AssetDatabase.GetAssetPath(  asset)  );
                return asset;
            }

            return null;
        }

#if UNITY_EDITOR
        [MenuItem( "NodeTSC/Config" )]
        static void OpenConfig()
        {
            Debug.Log( $"found : {AssetDatabase.OpenAsset( FindAsset() )}" );
        }
#endif
    }
}