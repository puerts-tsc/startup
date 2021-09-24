using Sirenix.Utilities;

namespace Runtime
{
    [GlobalConfig("Assets/Config/App/")]
    public class ConfigData : GlobalConfig<ConfigData>
    {
        public int MyGlobalVariable;
    }
}