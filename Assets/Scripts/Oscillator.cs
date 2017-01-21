using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
	public abstract class Oscillator : Modulator
	{
		public bool mute = false;
		public float freq = 440.0f;
		public float amp = 1.0f;

        public List<Modulator> freqModulators;
        public List<Modulator> phaseModulators;
        public List<Modulator> ampModulators;

        protected float phase = 0.0f;
		protected float sampleTime = 0.0f;
		protected float smoothFreq;

        private float smoothingSpeed = 1.0f;

        public void Start()
        {
            smoothFreq = freq;
            sampleTime = 1.0f / AudioSettings.outputSampleRate;
        }

		public float GetNextSample()
		{
			if (mute || sampleTime == 0.0f)
			{
				return 0.0f;
			}

			smoothFreq = Mathf.Lerp(smoothFreq, GetModulatedFreq(), smoothingSpeed);

			float f = smoothFreq * sampleTime;
			phase += f;
			phase -= Mathf.Floor(phase);

            return ComputeSample() * GetModulatedAmp();
		}

        protected float GetModulatedFreq()
        {
            float modulatedFreq = freq;

            foreach(Modulator freqModulator in freqModulators)
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

        protected abstract float ComputeSample();

        public override float GetValue()
        {
            return GetNextSample();
        }

        public override float GetPositiveValue()
        {
            return (GetNextSample() + amp) * 0.5f;
        }
    }
}
