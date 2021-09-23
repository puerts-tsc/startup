using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Blackboard")]
    [Description("Set a blackboard float variable at random between min and max value")]
    public class SetFloatRandom : ActionTask
    {

        public BBParameter<float> minValue;
        public BBParameter<float> maxValue;

        [BlackboardOnly]
        public BBParameter<float> floatVariable;

        protected override string info {
            get { return "Set " + floatVariable + " Random(" + minValue + ", " + maxValue + ")"; }
        }

        public override void OnExecute() {
            floatVariable.value = Random.Range(minValue.value, maxValue.value);
            EndAction();
        }
    }
}