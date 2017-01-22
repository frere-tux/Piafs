using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class Trigger : MonoBehaviour
    {
        public Envelope envelope;
        public float minTriggerTime;
        private float triggerTime;
        private float triggerDuration;
        public System.Action<Trigger,bool> onEndTrigger;
        public System.Action onStartTrigger;
        private bool triggered;

        void OnMouseDown()
        {
            triggered = true;
            envelope.Trigger();
            triggerTime = Time.time;
            if (onStartTrigger != null) onStartTrigger();
        }

        void Update()
        {
            if(Input.GetMouseButtonUp(0) && triggered)
            {
                triggered = false;
                envelope.Untrigger();
                triggerTime = Time.time - triggerDuration;
                if (onEndTrigger != null) onEndTrigger(this, triggerTime > minTriggerTime);
            }
        }
    }
}

