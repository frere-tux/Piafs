using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        public static Slot hoveredSlot;
        public static Brick draggedBrick;
        public static Slot draggedBrickOrigin;
        
        void Start()
        {
            instance = this;
        }

        public static void Drag(Brick brick)
        {
            if(instance == null)
            {
                Debug.LogError("Error : No Input Manager in scene !");
            }
            draggedBrick = brick;
            draggedBrickOrigin = brick.slot;
        }

        public static void Drop()
        {
            if(draggedBrick != null)
            {
                if (hoveredSlot)
                {
                    if(hoveredSlot.valid) 
                    {
                        if(hoveredSlot.SlottedBrick != null)
                        {
                            Brick b = hoveredSlot.SlottedBrick;
                            hoveredSlot.GrabOscillator();
                            b.Drop(b.LibrarySlot);
                        }
                        draggedBrick.Drop(hoveredSlot);
                        draggedBrick = null;
                        return;
                    }
                }
                draggedBrick.Drop(draggedBrick.LibrarySlot);
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

