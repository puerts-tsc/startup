using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    [Description("Action will end in Failure if no objects are found")]
    public class FindAllWithTag : ActionTask
    {

        [RequiredField, TagField]
        public BBParameter<string> searchTag = "Untagged";
        [BlackboardOnly]
        public BBParameter<List<GameObject>> saveAs;

        protected override string info {
            get { return "GetObjects '" + searchTag + "' as " + saveAs; }
        }

        public override void OnExecute() {
            saveAs.value = GameObject.FindGameObjectsWithTag(searchTag.value).ToList();
            EndAction(saveAs.value.Count != 0);
        }
    }
}