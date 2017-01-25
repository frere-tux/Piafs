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
        public Slot librarySlot;
        public Slot LibrarySlot { get { return librarySlot; } }
        public Slot rightSlot;

        Collider2D col;

        void Awake()
        {

            col = GetComponent<Collider2D>();
			slot.SetSlottedBrick(this);
            if (librarySlot == null) librarySlot = slot;
            if (rightSlot == null || !rightSlot.valid) rightSlot = librarySlot;
            if(librarySlot != slot)
            {
                slot.GrabOscillator();
                Drop(librarySlot);
            }
        }

        void OnValidate()
        {
            col = GetComponent<Collider2D>();
            if (librarySlot == null) librarySlot = slot;
        }

        public void OnMouseDown()
        {
            //Debug.Log("mousedown");
            InputManager.Drag(this);
            col.enabled = false;
            slot.GrabOscillator();
        }

        public void OnMouseEnter()
        {
            InputManager.hoveredSlot = slot;
        }

        public void OnMouseExit()
        {
            InputManager.hoveredSlot = null;
        }

        public void Drop(Slot _slot)
        {
            slot = _slot;
            col.enabled = true;
            SnapOn(slot);
            slot.DropBrick(this);
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

        public void GetDependencies(List<Modulator> result)
        {
            modulator.GetDependenciesRecursive(result);
        }
    }
}

