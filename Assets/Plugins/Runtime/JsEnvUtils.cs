using System;
using Puerts;

namespace Runtime
{
    public static partial class JsEnvUtils
    {
        public static JsEnv Env( this object target ) => JsMain.env;
        public static void JsCall<T>( this T target, object fn ) => Env( target ).Call( target, fn );
        public static void JsCall<T, T1>( this T target, object fn, T1 arg1 ) => Env( target ).Call( target, fn, arg1 );

        public static void JsCall<T, T1, T2>( this T target, object fn, T1 arg1, T2 arg2 ) =>
            Env( target ).Call( target, fn, arg1, arg2 );

        public static void JsCall<T, T1, T2, T3>( this T target, object fn, T1 arg1, T2 arg2, T3 arg3 ) =>
            Env( target ).Call( target, fn, arg1, arg2, arg3 );

        public static void JsCall<T, T1, T2, T3, T4>( this T target, object fn, T1 arg1, T2 arg2, T3 arg3, T4 arg4 ) =>
            Env( target ).Call( target, fn, arg1, arg2, arg3, arg4 );

        public static void JsCall<T, T1, T2, T3, T4, T5>( this T target, object fn, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
            T5 arg5 ) =>
            Env( target ).Call( target, fn, arg1, arg2, arg3, arg4, arg5 );
        public static bool IsDisposed( this JsEnv jsEnv ) => jsEnv == null || jsEnv.isolate == IntPtr.Zero;


        public static string GetFullName( this Enum myEnum )
        {
            return $"{myEnum.GetType().Name}.{myEnum.ToString()}";
        }

        const string GLOBAL_REQUIRE = "globalThis.$require"; 

        public static string AsString( this object obj ) => obj is Enum @enum ? @enum.GetFullName() : obj?.ToString();

        public static void Call<T>( this JsEnv jsEnv, T obj, object fn ) =>
            jsEnv.Eval<Action<T, string>>( GLOBAL_REQUIRE)?.Invoke( obj, fn.AsString() );

        public static void Call<T, T1>( this JsEnv jsEnv, T obj, object fn, T1 arg1 ) =>
            jsEnv.Eval<Action<T, string, T1>>( GLOBAL_REQUIRE )?.Invoke( obj, fn.AsString(), arg1 );

        public static void Call<T, T1, T2>( this JsEnv jsEnv, T obj, object fn, T1 arg1, T2 arg2 ) =>
            jsEnv.Eval<Action<T, string, T1, T2>>( GLOBAL_REQUIRE)?.Invoke( obj, fn.AsString(), arg1, arg2 );

        public static void Call<T, T1, T2, T3>( this JsEnv jsEnv, T obj, object fn, T1 arg1, T2 arg2, T3 arg3 ) =>
            jsEnv.Eval<Action<T, string, T1, T2, T3>>( GLOBAL_REQUIRE)?.Invoke( obj, fn.AsString(), arg1, arg2, arg3 );

        public static void Call<T, T1, T2, T3, T4>( this JsEnv jsEnv, T obj, object fn, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4 ) =>
            jsEnv.Eval<Action<T, string, T1, T2, T3, T4>>( GLOBAL_REQUIRE )
                ?.Invoke( obj, fn.AsString(), arg1, arg2, arg3, arg4 );

        public static void Call<T, T1, T2, T3, T4, T5>( this JsEnv jsEnv, T obj, object fn, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4, T5 arg5 ) =>
            jsEnv.Eval<Action<T, string, T1, T2, T3, T4, T5>>( GLOBAL_REQUIRE )
                ?.Invoke( obj, fn.AsString(), arg1, arg2, arg3, arg4, arg5 );
    }
}