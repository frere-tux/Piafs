using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class ModulatorBrick : MonoBehaviour
    {
        private float dragTime = 0.1f;
        private bool clicked = false;
        public ModulatorSlot slot;
        public Oscillator oscillator;
        Collider2D col;

        void Start()
        {
            col = GetComponent<Collider2D>();
        }

        public void OnMouseDown()
        {
            InputManager.Drag(this);
            col.enabled = false;
            slot.GrabOscillator();
        }

        public void Drop(ModulatorSlot _slot)
        {
            slot = _slot;
            col.enabled = true;
            SnapOn(slot);
            slot.DropOscillator(oscillator);
        }

        void SnapOn(ModulatorSlot _slot)
        {
            Vector3 originPos = _slot.transform.position;
            originPos.z = _slot.transform.position.z;
            transform.position = originPos;
        }
    }
}

