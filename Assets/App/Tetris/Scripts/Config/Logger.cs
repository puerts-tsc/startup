using System.Text;
using Sirenix.Utilities;
using UnityEngine;

namespace Tetris
{
    [GlobalConfig( "Assets/Config/Tetris" )]
    public class Logger : GlobalConfig<Logger>
    {
        private StringBuilder sb;

        public Logger()
        {
            sb = new StringBuilder();
        }

        public void Print()
        {
            Debug.Log( sb.ToString() );
        }

        public void Log( string log )
        {
            sb.Append( log + "\n" );
        }
    }
}