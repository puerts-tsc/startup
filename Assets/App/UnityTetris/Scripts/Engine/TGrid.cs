using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTetris
{
    public static class Plus
    {
        public static int ToInt( this float num ) => Mathf.RoundToInt(num);
    }

    [ Serializable ]
    public class TCell
    {
        public RectTransform rectTransform { get; set; }
        public TetriminoBlock block { get; set; }
        public int id { get; set; }
    }

    [ Serializable ]
    public class TGrid : List<TCell[]>
    {
        public static TGrid m_Instance;
        int Width = 9;

        public static TGrid instance {
            get {
                if (m_Instance == null) {
                    m_Instance = new TGrid();
                }

                return m_Instance;
            }
        }

        public TCell this[ Vector2 index ] {
            get => this[index.x, index.y];
            set => this[index.x, index.y] = value;
        }

        bool check( float col, float row, out (int x, int y) output )
        {
            output = ( col.ToInt(), row.ToInt() );
            int x = output.x, y = output.y;
            var max = this.Count + 5;
            while (this.Count <= y && this.Count <= max) this.Add(new TCell[this.Width]);
            return !( this.Count <= row ) && x < this.Width && x >= 0 && y >= 0;
        }

        public TCell this[ float col, float row ] {
            get => check(col, row, out var t) ? this[t.y][t.x] : null;
            set {
                if (check(col, row, out var t)) {
                    this[t.y][t.x] = value;
                }
            }
        }
    }
}