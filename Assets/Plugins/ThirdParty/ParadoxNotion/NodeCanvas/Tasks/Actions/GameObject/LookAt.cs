using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class LookAt : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> lookTarget;
        public bool repeat = false;

        protected override string info {
            get { return "LookAt " + lookTarget; }
        }

        public override void OnExecute() { DoLook(); }
        public override void OnUpdate() { DoLook(); }

        void DoLook() {
            var lookPos = lookTarget.value.transform.position;
            lookPos.y = agent.position.y;
            agent.LookAt(lookPos);

            if ( !repeat )
                EndAction(true);
        }
    }
}