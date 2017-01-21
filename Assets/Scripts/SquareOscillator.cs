using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [System.Serializable]
    public class SquareOscillator : Oscillator
    {
        protected override float ComputeSample()
        {
            return (GetModulatedPhase() >= 0.5f ? 1.0f : -1.0f) * amp;
        }
    }
}
