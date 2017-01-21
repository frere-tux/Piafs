using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [System.Serializable]
    public abstract class Oscillator
    {
        public bool mute = false;
        public float freq = 440.0f;
        public float amp = 1.0f;
        public float smoothingSpeed = 0.003f;
        public float freqModulation = 0.0f;

        protected float phase = 0.0f;
        protected float sampleTime = 0.0f;
        protected float smoothFreq;

        public void Init()
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

            smoothFreq = Mathf.Lerp(smoothFreq, freq + freqModulation, smoothingSpeed);

            float f = smoothFreq * sampleTime;
            phase += f;
            phase -= Mathf.Floor(phase);

            return ComputeSample();
        }

        protected abstract float ComputeSample();
    }
}
