using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Blackboard")]
    [Description("Stores the agent gameobject on the blackboard.")]
    public class GetSelf : ActionTask
    {

        [BlackboardOnly]
        public BBParameter<UnityEngine.GameObject> saveAs;

        public override void OnExecute() {
            saveAs.value = agent?.gameObject;
            EndAction(true);
        }
    }
}