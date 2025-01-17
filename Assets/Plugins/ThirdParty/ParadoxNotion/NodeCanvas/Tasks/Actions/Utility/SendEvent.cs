﻿using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Utility")]
    [Description("Send a graph event. If global is true, all graph owners in scene will receive this event. Use along with the 'Check Event' Condition")]
    public class SendEvent : ActionTask<GraphOwner>
    {

        [RequiredField]
        public BBParameter<string> eventName;
        public BBParameter<float> delay;
        public bool sendGlobal;

        protected override string info {
            get { return ( sendGlobal ? "Global " : "" ) + "Send Event [" + eventName + "]" + ( delay.value > 0 ? " after " + delay + " sec." : "" ); }
        }

        public override void OnUpdate() {
            if ( elapsedTime >= delay.value ) {
                if ( sendGlobal ) {
                    Graph.SendGlobalEvent(eventName.value, null, this);
                } else {
                    agent.SendEvent(eventName.value, null, this);
                }
                EndAction();
            }
        }
    }

    ///----------------------------------------------------------------------------------------------

    [Category("✫ Utility")]
    [Description("Send a graph event with T value. If global is true, all graph owners in scene will receive this event. Use along with the 'Check Event' Condition")]
    public class SendEvent<T> : ActionTask<GraphOwner>
    {

        [RequiredField]
        public BBParameter<string> eventName;
        public BBParameter<T> eventValue;
        public BBParameter<float> delay;
        public bool sendGlobal;

        protected override string info {
            get { return string.Format("{0} Event [{1}] ({2}){3}", ( sendGlobal ? "Global " : "" ), eventName, eventValue, ( delay.value > 0 ? " after " + delay + " sec." : "" )); }
        }

        public override void OnUpdate() {
            if ( elapsedTime >= delay.value ) {
                if ( sendGlobal ) {
                    Graph.SendGlobalEvent(eventName.value, eventValue.value, this);
                } else {
                    agent.SendEvent(eventName.value, eventValue.value, this);
                }
                EndAction();
            }
        }
    }
}