using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    [ExecuteInEditMode]
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
        public bool Triggered
        {
            get { return triggered; }
        }
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnMouseDown()
        {
            Activate();
        }

        public void Activate()
        {
            triggered = true;
            foreach (Modulator e in envelopes)
            {
                if(e!=null)e.Trigger();
            }
            triggerTime = Time.time;
            if (onStartTrigger != null) onStartTrigger();
            spriteRenderer.sprite = sprites[1];
        }

        public void Deactivate()
        {
            triggered = false;
            envelopes.ForEach(e => { if (e != null) e.Untrigger(); });
            triggerTime = Time.time - triggerDuration;
            if (onEndTrigger != null) onEndTrigger(this, triggerTime > minTriggerTime);
            spriteRenderer.sprite = sprites[0];
        }

        void Update()
        {
            if(Input.GetMouseButtonUp(0) && triggered)
            {
                Deactivate();
            }
        }
    }
}

