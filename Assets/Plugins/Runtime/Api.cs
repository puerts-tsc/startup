using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime
{
    public class Api : SerializedMonoBehaviour { }

    public class Api<T> : Api where T : Api<T>
    {
        static T m_Instance;

        public static T instance {
            get {
                if ( m_Instance == null && ( m_Instance = FindObjectOfType<T>() ) == null ) {
                    var go = JsMain.instance.transform.Find( typeof(T).Name )?.gameObject ??
                        new GameObject( typeof(T).Name ).Of( t => t.transform.SetParent( JsMain.instance.transform ) );
                    m_Instance = go.RequireComponent<T>();
                }

                return m_Instance;
            }
        }



        void Awake()
        {
            m_Instance ??= (T)this;
        }

        void OnDestroy()
        {
            if ( m_Instance == this ) m_Instance = null;
        }
    }
}

