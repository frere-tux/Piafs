using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [System.Serializable]
    [AddComponentMenu("Oscillator/Square Oscillator", -80)]
    public class SquareOscillator : Oscillator
    {
        protected override float ComputeSample()
        {
            return (GetModulatedPhase() >= 0.5f ? 1.0f : -1.0f);
        }
    }
}
