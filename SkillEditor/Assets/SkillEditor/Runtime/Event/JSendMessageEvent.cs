using UnityEngine;
using System.Collections;
namespace CySkillEditor
{
    [JFriendlyName("Send Message")]
    [JEvent("Signal/Send Message")]
    [JEventHideDuration()]
    public class JSendMessageEvent : JEventBase
    {
        public GameObject receiver = null;
        public string action = "OnSignal";

        public override void FireEvent()
        {
            //if(!Application.isPlaying)
            //	return;
            if (receiver)
                receiver.SendMessage(action);
            else
                Debug.LogWarning(string.Format("No receiver of signal \"{0}\" on object {1} ({2})", action, receiver.name, receiver.GetType().Name), receiver);
        }

        public override void ProcessEvent(float deltaTime)
        {

        }
    }
}