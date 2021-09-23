using System;

namespace Runtime {

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class AssemblyFilterAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class SubTypeFilterAttribute : Attribute { }


}
