using System;
using Puerts;
using UnityEngine;

namespace Examples.Node_tsc
{
    public class UIEvent : MonoBehaviour
    {
        static JsEnv jsEnv;

        void Start()
        {
            if ( jsEnv == null ) {
                jsEnv = new JsEnv();
                jsEnv.UsingAction<bool>(); //toggle.onValueChanged用到
            }

            var init = jsEnv.Eval<Action<MonoBehaviour>>( "require('UIEvent').init" );

            if ( init != null ) init( this );
        }
    }
}
