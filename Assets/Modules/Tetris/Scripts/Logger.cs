using System.Text;
using UnityEngine;

namespace Tetris
{
    public class Logger
    {
        private StringBuilder sb;

        public Logger()
        {
            sb = new StringBuilder();
        }

        public void Print()
        {
            Debug.Log(sb.ToString());
        }

        public void Log(string log)
        {
            sb.Append(log+"\n");
        }
    }
}
