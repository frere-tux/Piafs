using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Piafs
{
    public class LevelDescriptor : MonoBehaviour
    {
        public List<LevelControls> levelControls;
        public GameObject audioContent;

        public bool CheckVictory()
        {
            return levelControls.TrueForAll(control => control.IsSolved());
        }

        public void InstantiateBirdVersion()
        {
            int childCount = transform.childCount;
            for(int i = 0; i < childCount; i++)
            {
                transform.GetChild(i).GetComponent<Modulator>();
            }
        }
    }
}

