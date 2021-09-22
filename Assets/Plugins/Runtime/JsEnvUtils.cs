using System;
using Puerts;

namespace Plugins.Runtime
{
    public static partial class JsEnvUtils
    {
        public static bool IsDisposed( this JsEnv jsEnv ) => jsEnv == null || jsEnv.isolate == IntPtr.Zero;
    }
}