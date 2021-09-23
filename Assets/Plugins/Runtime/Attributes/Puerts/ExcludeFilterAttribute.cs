using System;

namespace Runtime {

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class ExcludeFilterAttribute : Attribute { }

}
