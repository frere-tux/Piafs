﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
	public abstract class Oscillator : Modulator
	{
		public bool mute = false;

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
