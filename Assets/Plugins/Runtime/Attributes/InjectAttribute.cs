using System;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace Runtime {

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class InjectAttribute : Attribute {

    public string name;
    public Type type;

    public InjectAttribute(string _name)
    {
        name = _name;
    }

    public InjectAttribute(Type _type, string _name = null)
    {
        type = _type;

        if (!_name.IsNullOrWhitespace()) {
            name = _name;
        }
    }

    public static void Dispatch()
    {
        var changed = false;

        Assembly.GetExecutingAssembly()
            .GetCustomAttributes<InjectAttribute>()
            .ForEach(t => {
                Debug.Log(t.type);
            });
    }

}

}