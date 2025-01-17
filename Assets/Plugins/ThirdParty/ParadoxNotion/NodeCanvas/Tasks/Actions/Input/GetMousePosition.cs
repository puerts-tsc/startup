using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("Input")]
    public class GetMousePosition : ActionTask
    {

        [BlackboardOnly]
        public BBParameter<Vector3> saveAs;
        public bool repeat;
        public override void OnExecute() { Do(); }
        public override void OnUpdate() { Do(); }

        void Do() {
            saveAs.value = Input.mousePosition;
            if ( !repeat )
                EndAction();
        }
    }
}