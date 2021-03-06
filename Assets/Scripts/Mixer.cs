﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [ExecuteInEditMode]
    [AddComponentMenu("Modulator/Mixer", -80)]
    public class Mixer : Modulator
    {
        public List<Modulator> modulators;

        public override float GetValue()
        {
            float mixedValue = 0.0f;
            foreach (Modulator oscillator in modulators)
            {
                if(oscillator != null)mixedValue += oscillator.GetValue();
            }

            return mixedValue * GetModulatedAmp();
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }

        public override bool Compare(Modulator other)
        {
            if (!base.Compare(other))
                return false;

            if (modulators.Count != ((Mixer)other).modulators.Count)
            {
                return false;
            }

            for (int i = 0; i < modulators.Count; ++i)
            {
                if (!modulators[i].Compare(((Mixer)other).modulators[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
