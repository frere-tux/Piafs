using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class Trigger : MonoBehaviour
    {
        public Envelope envelope;
        public float minTriggerTime;
        public Sprite[] sprites;

        private float triggerTime;
        private float triggerDuration;
        public System.Action<Trigger,bool> onEndTrigger;
        public System.Action onStartTrigger;
        private bool triggered;
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnMouseDown()
        {
            triggered = true;
            envelope.Trigger();
            triggerTime = Time.time;
            if (onStartTrigger != null) onStartTrigger();
            spriteRenderer.sprite = sprites[1];
            
        }

        void Update()
        {
            if(Input.GetMouseButtonUp(0) && triggered)
            {
                triggered = false;
                envelope.Untrigger();
                triggerTime = Time.time - triggerDuration;
                if (onEndTrigger != null) onEndTrigger(this, triggerTime > minTriggerTime);
                spriteRenderer.sprite = sprites[0];
            }
        }
    }
}

