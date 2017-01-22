using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class Brick : LevelControls
    {
        [Header("-- Runtime --")]
        public Slot slot;
        public Modulator modulator;
        [Header("-- Level design --")]
        public Slot rightSlot;

        Collider2D col;

        void Start()
        {
            col = GetComponent<Collider2D>();
            SnapOn(slot);
        }

        public void OnMouseDown()
        {
            //Debug.Log("mousedown");
            InputManager.Drag(this);
            col.enabled = false;
            slot.GrabOscillator();
        }

        public void Drop(Slot _slot)
        {
            slot = _slot;
            col.enabled = true;
            SnapOn(slot);
            slot.DropOscillator(modulator);
        }

        void SnapOn(Slot _slot)
        {
            Vector3 originPos = _slot.transform.position;
            originPos.z = this.transform.position.z;
            transform.position = originPos;
        }

        public override bool IsSolved()
        {
            return slot == rightSlot;
        }
    }
}

