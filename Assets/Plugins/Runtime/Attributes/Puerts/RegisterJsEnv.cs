using System;

namespace Runtime {

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class RegisterEnvAttribute : Attribute {

    public int Order { get; set; }
    public RegisterEnvAttribute() { }

    public RegisterEnvAttribute(int order)
    {
        Order = order;
    }

}

}