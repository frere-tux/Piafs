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
            if (ampModulators.Count == 0) return 0f;
            float multipliedAmp = amp;
            foreach (Modulator ampModulator in ampModulators)
            {
                if(ampModulator != null)multipliedAmp *= ampModulator.GetPositiveValue();
            }
            return multipliedAmp;
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }

    }
}

