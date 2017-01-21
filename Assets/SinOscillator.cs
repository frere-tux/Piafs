using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [System.Serializable]
    public class CurveOscillatorSinOscillator : Oscillator
    {
        protected override float ComputeSample()
        {
            return Mathf.Sin(ModulatedPhase * 2.0f * 3.1415926f) * amp;
        }
    }
}
