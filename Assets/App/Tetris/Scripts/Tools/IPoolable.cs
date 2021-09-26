using UnityEngine;

namespace Tetris.Tools
{
    public interface IPoolable<T>  where T : Tetris<T>, IPoolable<T> {
        /// <summary>
        /// aready in pool？
        /// </summary>
        /// <value></value>
        bool InPooled { get; set; }
        /// <summary>
        ///whitch pool does this object belong to
        /// </summary>
        /// <value></value>
        Pool<T> Pool { get; set; }
    }
}