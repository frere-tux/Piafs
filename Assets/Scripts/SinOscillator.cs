using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [ExecuteInEditMode]
    [AddComponentMenu("Oscillator/Sin Oscillator", -80)]
    public class SinOscillator : Oscillator
    {
        protected override float ComputeSample()
        {
            return Mathf.Sin(GetModulatedPhase() * 2.0f * 3.1415926f);
        }
    }
}
