using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Piafs
{
    [AddComponentMenu("Modulator/White noise", -80)]
    public class WhiteNoise : Modulator
    {
        private System.Random randomizer = new System.Random();
        private float value = 0.0f;

        public override float GetValue()
        {
            double randValue = randomizer.NextDouble();
            value = ((float)randValue * 2.0f - 1.0f) * GetModulatedAmp();
            return value;
        }

        public override float GetPositiveValue()
        {
            double randValue = randomizer.NextDouble();
            value = (float)randValue * GetModulatedAmp();
            return value;
        }

        public override float GetLastPositiveValue()
        {
            return value;
        }
    }
}
