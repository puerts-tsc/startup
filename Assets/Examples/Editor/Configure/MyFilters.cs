using System;
using System.Linq;
using System.Reflection;
using Puerts;
using Runtime;
using Sirenix.Utilities;

namespace Examples.Configure
{
    [Configure]
    public class MyFilters
    {
        static string[] type_BlackList =>
            new string[] {
                //[ 类名 ]不能包含里面的字符
                "Editor",
            };

        static Type[] types =>
            new Type[] {
                //[ 类名 ]
            };

        static string[] fullname_BlackList =>
            new string[] {
                //[ 类名.方法名 ]不能包含里面的字符
            };

        // 不能包含特性
        static Type[] attribute_Blacklist =>
            new[] {
                typeof(PuertsIgnoreAttribute),
                //typeof(ObsoleteAttribute)
            };

        [Filter]
        static bool Filter( MemberInfo memberInfo )
        {
            if ( attribute_Blacklist.Any( t => memberInfo.GetType().IsDefined( t, true ) ||
                memberInfo.DeclaringType?.IsDefined( t, true ) == true ) ) {
                return true;
            }

            var typename = $"{memberInfo.DeclaringType?.GetNiceFullName()}"; 
            if ( type_BlackList.Any( s => typename.Contains( s ) ) ) {
                return true;
            }

            if ( memberInfo.DeclaringType != null &&
                types.Any( t => t.IsAssignableFrom( memberInfo.DeclaringType ) ) ) {
                return true;
            }

            if ( fullname_BlackList.Any( s => $"{typename}.{memberInfo.Name}".Contains( s ) ) ) {
                return true;
            }

            return false;
        }
    }
}