using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        public static ModulatorSlot hoveredSlot;
        public static ModulatorBrick draggedBrick;
        public static ModulatorSlot draggedBrickOrigin;
        
        void Start()
        {
            instance = this;
        }

        public static void Drag(ModulatorBrick brick)
        {
            draggedBrick = brick;
            draggedBrickOrigin = brick.slot;
        }

        public static void Drop()
        {
            if(draggedBrick != null)
            {
                if (hoveredSlot)
                {
                    draggedBrick.Drop(hoveredSlot);
                }
                else
                {
                    draggedBrick.Drop(draggedBrickOrigin);
                }
                draggedBrick = null;
            }
        }


        
        void Update()
        {
            if(draggedBrick != null)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = draggedBrick.transform.position.z;
                draggedBrick.transform.position = mouseWorldPos;

                if (Input.GetMouseButtonUp(0))
                {
                    Drop();
                }
            }
            
        }
    }
}

