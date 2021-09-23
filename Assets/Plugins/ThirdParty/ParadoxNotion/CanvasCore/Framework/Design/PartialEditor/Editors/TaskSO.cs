using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace NodeCanvas.Editor
{
    public class TaskSO : SerializedScriptableObject
    {
        [OdinSerialize]
        public Task m_节点数据;
    }
}