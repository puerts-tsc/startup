using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.DialogueTrees
{

    /// Use DialogueTrees to create Dialogues between Actors
    [GraphInfo(
        packageName = "NodeCanvas",
        docsURL = "http://nodecanvas.paradoxnotion.com/documentation/",
        resourcesURL = "http://nodecanvas.paradoxnotion.com/downloads/",
        forumsURL = "http://nodecanvas.paradoxnotion.com/forums-page/"
        )]
    [CreateAssetMenu(menuName = "Custom/ParadoxNotion/NodeCanvas/Dialogue Tree Asset")]
    public class DialogueTree : Graph
    {

        ///----------------------------------------------------------------------------------------------
        [System.Serializable]
        class DerivedSerializationData
        {
            public List<ActorParameter> actorParameters;
        }

        public override object OnDerivedDataSerialization() {
            var data = new DerivedSerializationData();
            data.actorParameters = this.actorParameters;
            return data;
        }

        public override void OnDerivedDataDeserialization(object data) {
            if ( data is DerivedSerializationData ) {
                this.actorParameters = ( (DerivedSerializationData)data ).actorParameters;
            }
        }
        ///----------------------------------------------------------------------------------------------

        ///An Actor Parameter
        [System.Serializable]
        public class ActorParameter
        {
            [SerializeField] private string _keyName;
            [SerializeField] private string _id;
            [SerializeField] private UnityEngine.Object _actorObject;
            [System.NonSerialized] private IDialogueActor _actor;

            ///Key name of the parameter
            public string name {
                get { return _keyName; }
                set { _keyName = value; }
            }

            ///ID of the parameter
            public string ID => string.IsNullOrEmpty(_id) ? _id = System.Guid.NewGuid().ToString() : _id;

            ///The reference actor of the parameter
            public IDialogueActor actor {
                get
                {
                    if ( _actor == null ) {
                        _actor = _actorObject as IDialogueActor;
                    }
                    return _actor;
                }
                set
                {
                    _actor = value;
                    _actorObject = value as UnityEngine.Object;
                }
            }

            public ActorParameter() { }
            public ActorParameter(string name) { this.name = name; }
            public ActorParameter(string name, IDialogueActor actor) {
                this.name = name;
                this.actor = actor;
            }

            public override string ToString() { return name; }
        }

        ///----------------------------------------------------------------------------------------------

        ///The string used for "Instigator"
        public const string INSTIGATOR_NAME = "INSTIGATOR";

        ///The dialogue actor parameters. We let Unity serialize this as well
        [SerializeField] public List<ActorParameter> actorParameters = new List<ActorParameter>();

        public static event Action<DialogueTree> OnDialogueStarted;
        public static event Action<DialogueTree> OnDialoguePaused;
        public static event Action<DialogueTree> OnDialogueFinished;
        public static event Action<SubtitlesRequestInfo> OnSubtitlesRequest;
        public static event Action<MultipleChoiceRequestInfo> OnMultipleChoiceRequest;

        ///The current DialoguTree running
        public static DialogueTree currentDialogue { get; private set; }
        ///The previous DialoguTree running
        public static DialogueTree previousDialogue { get; private set; }

        ///The current node of this DialogueTree
        public DTNode currentNode { get; private set; }

        ///----------------------------------------------------------------------------------------------
        public override System.Type baseNodeType => typeof(DTNode);
        public override bool requiresAgent => false;
        public override bool requiresPrimeNode => true;
        public override bool isTree => true;
        public override bool allowBlackboardOverrides => true;
        sealed public override bool canAcceptVariableDrops => false;
        ///----------------------------------------------------------------------------------------------

        ///A list of the defined names for the involved actor parameters
        public List<string> definedActorParameterNames {
            get
            {
                var list = actorParameters.Select(r => r.name).ToList();
                list.Insert(0, INSTIGATOR_NAME);
                return list;
            }
        }

        ///Returns the ActorParameter by id
        public ActorParameter GetParameterByID(string id) {
            return actorParameters.Find(p => p.ID == id);
        }

        ///Returns the ActorParameter by name
        public ActorParameter GetParameterByName(string paramName) {
            return actorParameters.Find(p => p.name == paramName);
        }

        ///Returns the actor by parameter id.
        public IDialogueActor GetActorReferenceByID(string id) {
            var param = GetParameterByID(id);
            return param != null ? GetActorReferenceByName(param.name) : null;
        }

        ///Resolves and gets an actor based on the key name
        public IDialogueActor GetActorReferenceByName(string paramName) {

            //Check for INSTIGATOR selection
            if ( paramName == INSTIGATOR_NAME ) {

                //return it directly if it implements IDialogueActor
                if ( agent is IDialogueActor ) {
                    return (IDialogueActor)agent;
                }

                //Otherwise use the default actor and set name and transform from agent
                if ( agent != null ) {
                    return new ProxyDialogueActor(agent.gameObject.name, agent.transform);
                }

                return new ProxyDialogueActor("Null Instigator", null);
            }

            //Check for non INSTIGATOR selection. If there IS an actor reference return it
            var refData = actorParameters.Find(r => r.name == paramName);
            if ( refData != null && refData.actor != null ) {
                return refData.actor;
            }

            //Otherwise use the default actor and set the name to the key and null transform
            Logger.Log(string.Format("An actor entry '{0}' on DialogueTree has no reference. A dummy Actor will be used with the entry Key for name", paramName), "Dialogue Tree", this);
            return new ProxyDialogueActor(paramName, null);
        }


        ///Set the target IDialogueActor for the provided key parameter name
        public void SetActorReference(string paramName, IDialogueActor actor) {
            var param = actorParameters.Find(p => p.name == paramName);
            if ( param == null ) {
                Logger.LogError(string.Format("There is no defined Actor key name '{0}'", paramName), "Dialogue Tree", this);
                return;
            }
            param.actor = actor;
        }

        ///Set all target IDialogueActors at once by provided dictionary
        public void SetActorReferences(Dictionary<string, IDialogueActor> actors) {
            foreach ( var pair in actors ) {
                var param = actorParameters.Find(p => p.name == pair.Key);
                if ( param == null ) {
                    Logger.LogWarning(string.Format("There is no defined Actor key name '{0}'. Seting actor skiped", pair.Key), "Dialogue Tree", this);
                    continue;
                }
                param.actor = pair.Value;
            }
        }

        ///Continues the DialogueTree at provided child connection index of currentNode
        public void Continue(int index = 0) {
            if ( index < 0 || index > currentNode.outConnections.Count - 1 ) {
                Stop(true);
                return;
            }
            currentNode.outConnections[index].status = Status.Success; //editor vis
            EnterNode((DTNode)currentNode.outConnections[index].targetNode);
        }

        ///Enters the provided node
        public void EnterNode(DTNode node) {
            currentNode = node;
            currentNode.Reset(false);
            if ( currentNode.Execute(agent, blackboard) == Status.Error ) {
                Stop(false);
            }
        }

        ///Raise the OnSubtitlesRequest event
        public static void RequestSubtitles(SubtitlesRequestInfo info) {
            if ( OnSubtitlesRequest != null )
                OnSubtitlesRequest(info);
            else Logger.LogWarning("Subtitle Request event has no subscribers. Make sure to add the default '@DialogueGUI' prefab or create your own GUI.", "Dialogue Tree");
        }

        ///Raise the OnMultipleChoiceRequest event
        public static void RequestMultipleChoices(MultipleChoiceRequestInfo info) {
            if ( OnMultipleChoiceRequest != null )
                OnMultipleChoiceRequest(info);
            else Logger.LogWarning("Multiple Choice Request event has no subscribers. Make sure to add the default '@DialogueGUI' prefab or create your own GUI.", "Dialogue Tree");
        }

        protected override void OnGraphStarted() {
            previousDialogue = currentDialogue;
            currentDialogue = this;

            Logger.Log(string.Format("Dialogue Started '{0}'", this.name), "Dialogue Tree", this);
            if ( OnDialogueStarted != null ) {
                OnDialogueStarted(this);
            }

            if ( !( agent is IDialogueActor ) ) {
                Logger.Log("INSTIGATOR agent used in DialogueTree does not implement IDialogueActor. A dummy actor will be used.", "Dialogue Tree", this);
            }

            currentNode = currentNode != null ? currentNode : (DTNode)primeNode;
            EnterNode(currentNode);
        }

        protected override void OnGraphUpdate() {
            if ( currentNode is IUpdatable ) {
                ( currentNode as IUpdatable ).Update();
            }
        }

        protected override void OnGraphStoped() {
            currentDialogue = previousDialogue;
            previousDialogue = null;
            currentNode = null;

            Logger.Log(string.Format("Dialogue Finished '{0}'", this.name), "Dialogue Tree", this);
            if ( OnDialogueFinished != null ) {
                OnDialogueFinished(this);
            }
        }

        protected override void OnGraphPaused() {
            Logger.Log(string.Format("Dialogue Paused '{0}'", this.name), "Dialogue Tree", this);
            if ( OnDialoguePaused != null ) {
                OnDialoguePaused(this);
            }
        }

        protected override void OnGraphUnpaused() {
            currentNode = currentNode != null ? currentNode : (DTNode)primeNode;
            EnterNode(currentNode);

            Logger.Log(string.Format("Dialogue Resumed '{0}'", this.name), "Dialogue Tree", this);
            if ( OnDialogueStarted != null ) {
                OnDialogueStarted(this);
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Dialogue Tree Object", false, 0)]
        static void Editor_CreateGraph() {
            var dt = new GameObject("DialogueTree").AddComponent<DialogueTreeController>();
            UnityEditor.Selection.activeObject = dt;
        }
#endif

    }
}