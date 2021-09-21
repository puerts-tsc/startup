using System.IO;
using System.Linq;
using Admin;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Runtime
{
    [CreateAssetMenu( fileName = "TsConfig", menuName = "Node-tsc/TsConfig", order = 0 )]
    public class TsConfig : ScriptableObject
    {
        static TsConfig m_Instance;
        public static TsConfig instance => m_Instance ?? ( m_Instance = FindAsset() );

        [SerializeField]
        [Header( "编译后的js文件目录" )]
        public Object m_CompileDir;

        [SerializeField, HideInInspector]
        [Header( "编译路径" ), ReadOnly]
        string m_OutputPath;

        public string OutputPath {
            get => m_OutputPath;
            set => m_OutputPath = value;
        }

        [Header( "ts项目目录" ), SerializeField]
        public Object TsRoot;

        [SerializeField]
        [Header( "Inspector 调试接口" )]
        int m_Port = 9229;

        public int Port {
            get => m_Port;
            set => m_Port = value;
        }

        [SerializeField]
        public string m_browserPath;

        [SerializeField]
        [Header( "脚本前缀(前缀应该和脚本的tag相同)" )]
        public AssetLabelReference[] Prefix;

        void OnEnable()
        {
            if ( instance == null ) {
                m_Instance = this;
                Debug.Log( $"TsConfig Loaded. {this.OutputPath}" );
            }
        }

        void OnDisable()
        {
            if ( instance == this ) {
                m_Instance = null;
            }
        }

        [Button]
        void 打开Inspector监听窗口() =>
            MyProcess.Run( m_browserPath,
                $"devtools://devtools/bundled/inspector.html?v8only=true&ws=127.0.0.1:{m_Port}" );

        void OnValidate()
        {
#if UNITY_EDITOR
            if ( m_CompileDir != null ) {
                m_OutputPath = AssetDatabase.GetAssetPath( m_CompileDir );
            }
#endif
        }

        public static TsConfig FindAsset()
        {
#if UNITY_EDITOR
            return AssetDatabase.FindAssets( $"t:{typeof(TsConfig).Name}" ).Select( guid =>
                AssetDatabase.LoadAssetAtPath<TsConfig>( AssetDatabase.GUIDToAssetPath( guid ) ) ).FirstOrDefault();
#endif
            return ScriptableObject.CreateInstance<TsConfig>();
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