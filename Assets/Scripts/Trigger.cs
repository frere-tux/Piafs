using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class Trigger : MonoBehaviour
    {
        public List<Modulator> envelopes;
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
            foreach (Modulator e in envelopes)
            {
                e.Trigger();
            }
            triggerTime = Time.time;
            if (onStartTrigger != null) onStartTrigger();
            spriteRenderer.sprite = sprites[1];
            
        }

        void Update()
        {
            if(Input.GetMouseButtonUp(0) && triggered)
            {
                triggered = false;
                envelopes.ForEach(e=>e.Untrigger());
                triggerTime = Time.time - triggerDuration;
                if (onEndTrigger != null) onEndTrigger(this, triggerTime > minTriggerTime);
                spriteRenderer.sprite = sprites[0];
            }
        }
    }
}

