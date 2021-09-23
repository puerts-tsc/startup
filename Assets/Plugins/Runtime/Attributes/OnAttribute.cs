using System;

namespace Runtime {

[AttributeUsage(AttributeTargets.All)]
public class OnAttribute : Attribute {

    public OnAttribute(Type type)
    {
        this.type = type;
    }

    public Type type { get; set; }

}

}