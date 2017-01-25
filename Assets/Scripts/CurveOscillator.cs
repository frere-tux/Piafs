using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [ExecuteInEditMode]
    [System.Serializable]
    [AddComponentMenu("Oscillator/Curve Oscillator", -80)]
    public class CurveOscillator : Oscillator
    {
        public AnimationCurve curve;

        protected override float ComputeSample()
        {
            return curve.Evaluate(GetModulatedPhase());
        }
    }
}
