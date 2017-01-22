using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public class TestCompare : MonoBehaviour
    {
        public Modulator modulator1;
        public Modulator modulator2;

        private bool equal = false;
        private bool firstPass = true;

        void Update()
        {
            if (modulator1 == null || modulator2 == null)
            {
                return;
            }

            bool oldEqual = equal;
            equal = modulator1.Compare(modulator2);
            if (equal != oldEqual || firstPass)
            {
                Color backgroundColor;
                if (equal)
                {
                    backgroundColor = Color.green;
                }
                else
                {
                    backgroundColor = Color.red;
                }

                Camera.main.backgroundColor = backgroundColor;

                firstPass = false;
            }
        }
    }
}
