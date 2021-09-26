using Sirenix.OdinInspector;
using UnityEngine;

namespace Tetris
{
    public class TetrisComponent : SerializedMonoBehaviour { }

    public class Tetris<T> : TetrisComponent where T : Tetris<T>
    {
        static T m_Instance;
        public static bool hasInstance => m_Instance != null;

        public static T instance {
            get {
                if ( m_Instance == null ) {
                    m_Instance = FindObjectOfType<T>( true );
                }

                return m_Instance;
            }
            set => m_Instance = value;
        }

        protected void Awake()
        {
            m_Instance ??= (T)this;
        }
    }
}