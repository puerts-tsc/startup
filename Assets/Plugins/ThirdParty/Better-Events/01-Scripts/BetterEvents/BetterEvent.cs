using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BetterEvents
{
    [Serializable]
    public struct BetterEvent
    {
        [HideReferenceObjectPicker,
         ListDrawerSettings( CustomAddFunction = "GetDefaultBetterEvent", OnTitleBarGUI = "DrawInvokeButton" )]
        public List<BetterEventEntry> Events;

        UnityEvent m_UnityEvent;

        public UnityEvent uEvent {
            get => m_UnityEvent ??= new UnityEvent();
            set => m_UnityEvent = value;
        }

        public void Invoke()
        {
            if ( this.Events == null ) {
                uEvent.Invoke();
                return;
            }

            for ( int i = 0; i < this.Events.Count; i++ ) {
                this.Events[i].Invoke();
            }
            uEvent.Invoke();
        }

#if UNITY_EDITOR

        private BetterEventEntry GetDefaultBetterEvent()
        {
            return new BetterEventEntry( null );
        }

        private void DrawInvokeButton()
        {
            if ( Sirenix.Utilities.Editor.SirenixEditorGUI.ToolbarButton( "Invoke" ) ) {
                this.Invoke();
            }
        }

#endif
    }
}