using Puerts;

namespace Examples.Configure
{
    [Configure]
    public class GenPath
    {
        [CodeOutputDirectory]
        static string CodeOutputPath => UnityEngine.Application.dataPath + "/Gen/";
    }
}