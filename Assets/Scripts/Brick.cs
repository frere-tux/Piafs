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
        private Vector3 rawPosition;

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
            slot.SetSlottedBrick(this);
            if (librarySlot == null) librarySlot = slot;
        }

        void Update()
        {
            transform.localPosition = Toolkit.Damp(transform.localPosition, rawPosition, 0.99f,Time.deltaTime);
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
			Vector3 posSnapshot = transform.position; ;
            transform.position = originPos;
			rawPosition = transform.localPosition;
			if (_slot == librarySlot) transform.position = posSnapshot;
        }

		public void SetPositionHard(Vector3 _pos)
		{
			transform.position = _pos;
			rawPosition = transform.localPosition;
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

