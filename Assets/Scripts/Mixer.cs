using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public class Mixer : Modulator
    {
        public List<Modulator> oscillators;

        public override float GetValue()
        {
            float mixedValue = 0.0f;
            foreach (Modulator oscillator in oscillators)
            {
                mixedValue += oscillator.GetValue();
            }

            return mixedValue;
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }
    }
}
