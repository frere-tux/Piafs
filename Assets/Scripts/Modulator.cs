using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public abstract class Modulator : MonoBehaviour
    {
        public float freq = 1.0f;
        public float amp = 1.0f;
        protected float phase = 0.0f;

        public List<Modulator> freqModulators;
        public List<Modulator> phaseModulators;
        public List<Modulator> ampModulators;

        public abstract float GetValue();
        public abstract float GetPositiveValue();

        protected float GetModulatedFreq()
        {
            float modulatedFreq = freq;

            foreach (Modulator freqModulator in freqModulators)
            {
                modulatedFreq += freqModulator.GetValue();
            }

            return modulatedFreq;
        }

        protected float GetModulatedPhase()
        {
            float modulatedPhase = phase;

            foreach (Modulator phaseModulator in phaseModulators)
            {
                modulatedPhase += phaseModulator.GetValue();
            }

            return modulatedPhase;
        }

        protected float GetModulatedAmp()
        {
            float modulatedAmp = amp;

            foreach (Modulator ampModulator in ampModulators)
            {
                modulatedAmp += ampModulator.GetPositiveValue();
            }

            return modulatedAmp;
        }
    }
}
