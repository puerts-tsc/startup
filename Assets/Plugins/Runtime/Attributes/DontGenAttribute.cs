using System;

namespace Runtime {

[AttributeUsage(AttributeTargets.All)]
public class DontGenAttribute : Attribute {

    public DontGenAttribute() { }

}

}