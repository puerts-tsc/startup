using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Tools
{
    /// <summary>
    /// C# object pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : new () {
        private Stack<T> m_Objects = new Stack<T> ();
        private bool m_FixedSize;
        private int m_PoolSize;

        public ObjectPool (int poolSize = 10, bool fixedSize = true) {

            this.m_PoolSize = poolSize;
            this.m_FixedSize = fixedSize;

            for (int i = 0; i < poolSize; i++) {
                m_Objects.Push (new T ());
            }
        }

        public T New () {
            T item = default (T);
            if (m_Objects.Count > 0) {
                item = m_Objects.Pop ();
            } else {

                if (!m_FixedSize) {
                    m_PoolSize++;
                } else {
                    Debug.LogWarning ("No object avaliable,Create new Instance!");
                }

                item = new T ();
            }
            return item;
        }

        public void Free (T item) {
            if (!m_FixedSize || m_PoolSize > m_Objects.Count) {
                m_Objects.Push (item);
            } else {
                Debug.LogWarning ("Instance can't return to Pool,wait GC");
            }
        }
    }
}