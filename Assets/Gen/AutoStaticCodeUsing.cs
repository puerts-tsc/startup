namespace PuertsStaticWrap
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using JsEnv = Puerts.JsEnv;
    using BindingFlags = System.Reflection.BindingFlags;

    public static class AutoStaticCodeUsing
    {
        public static void AutoUsing(this JsEnv jsEnv)
        {
            jsEnv.UsingAction<FlowCanvas.Flow>();
            jsEnv.UsingAction<NodeCanvas.BehaviourTrees.BehaviourTree, NodeCanvas.Framework.Status>();
            jsEnv.UsingAction<NodeCanvas.Framework.Status>();
            jsEnv.UsingAction<System.Boolean>();
            jsEnv.UsingAction<System.Boolean, System.Boolean, System.Int32>();
            jsEnv.UsingAction<System.Int32>();
            jsEnv.UsingAction<System.Int32, System.Int32, System.Int32>();
            jsEnv.UsingAction<System.Int32, UnityEngine.Vector2>();
            jsEnv.UsingAction<System.IntPtr, Puerts.ISetValueToJs, System.IntPtr, System.Object>();
            jsEnv.UsingAction<System.Single>();
            jsEnv.UsingAction<UnityEngine.CustomRenderTexture, System.Int32>();
            jsEnv.UsingAction<UnityEngine.Vector2>();
            jsEnv.UsingFunc<System.Boolean>();
            jsEnv.UsingFunc<System.Int32>();
            jsEnv.UsingFunc<System.Int32, System.Boolean>();
            jsEnv.UsingFunc<System.Int32, System.Int32, System.Int32>();
            jsEnv.UsingFunc<System.IntPtr, Puerts.IGetValueFromJs, System.IntPtr, System.Boolean, System.Object>();
            jsEnv.UsingFunc<System.Reflection.Assembly, System.String, System.Boolean, System.Type>();
            jsEnv.UsingFunc<System.Reflection.FieldInfo, System.Boolean>();
            jsEnv.UsingFunc<System.Reflection.MemberInfo, System.Object, System.Boolean>();
            jsEnv.UsingFunc<System.Single>();
            jsEnv.UsingFunc<System.Single, System.Single>();
            jsEnv.UsingFunc<System.Single, System.Single, System.Single>();
            jsEnv.UsingFunc<System.Single, System.Single, System.Single, System.Single>();
            jsEnv.UsingFunc<System.Type, System.Object, System.Boolean>();
            jsEnv.UsingFunc<System.ValueTuple<System.Single, System.Single>>();
            jsEnv.UsingFunc<UnityEngine.UI.ILayoutElement, System.Single>();
            
        }
        public static void UsingAction(this JsEnv jsEnv, params string[] args)
        {
            jsEnv.UsingGeneric(true, FindTypes(args));
        }
        public static void UsingFunc(this JsEnv jsEnv, params string[] args)
        {
            jsEnv.UsingGeneric(false, FindTypes(args));
        }
        public static void UsingGeneric(this JsEnv jsEnv, bool usingAction, params Type[] types)
        {
            var name = usingAction ? "UsingAction" : "UsingFunc";
            var count = types.Length;
            var method = (from m in typeof(JsEnv).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                          where m.Name.Equals(name)
                             && m.IsGenericMethod
                             && m.GetGenericArguments().Length == count
                          select m).FirstOrDefault();
            if (method == null)
                throw new Exception("Not found method: '" + name + "', ArgsLength=" + count);
            method.MakeGenericMethod(types).Invoke(jsEnv, null);
        }
        static Type[] FindTypes(string[] args)
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            var types = new List<Type>();
            foreach (var arg in args)
            {
                Type type = null;
                for (var i = 0; i < assemblys.Length && type == null; i++)
                    type = assemblys[i].GetType(arg, false);
                if (type == null)
                    throw new Exception("Not found type: '" + arg + "'");
                types.Add(type);
            }
            return types.ToArray();
        }
    }
}