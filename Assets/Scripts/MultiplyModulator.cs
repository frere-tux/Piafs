using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    [ExecuteInEditMode]
    public class MultiplyModulator : Modulator
    {
        public override float GetValue()
        {
            float multipliedAmp = amp;
            foreach (Modulator ampModulator in ampModulators)
            {
                multipliedAmp *= ampModulator.GetPositiveValue();
            }
            return multipliedAmp;
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }

    }
}

