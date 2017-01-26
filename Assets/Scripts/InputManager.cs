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
        public static float brickZ;
        
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
            brickZ = draggedBrick.transform.position.z;
        }

        public static void Drop()
        {
            if(draggedBrick != null)
            {
                Vector3 brickPos = draggedBrick.transform.position;
                brickPos.z = brickZ;
                draggedBrick.transform.position = brickPos;
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
                mouseWorldPos.z = brickZ - 0.1f;
				mouseWorldPos.y = Mathf.Max(draggedBrick.librarySlot.transform.position.y - 0.1f, mouseWorldPos.y);
                draggedBrick.SetPositionHard(mouseWorldPos);

                if (Input.GetMouseButtonUp(0))
                {
                    Drop();
                }
            }
            
        }
    }
}

