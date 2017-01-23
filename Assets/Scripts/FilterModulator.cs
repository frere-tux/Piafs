using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    [AddComponentMenu("Modulator/Filter Modulator (WIP)", -80)]
    public class FilterModulator :  Modulator
    {
        public float cutoff;
        private List<float> buffer;
        public bool bypass;
        [Range(1, 1023)]
        public int smoothLength;

        protected override void Start()
        {
            base.Start();
            buffer = new List<float>(1024);
            for (int i = 0; i < buffer.Capacity; i++) buffer.Add(0f);
        }

        public override float GetValue()
        {
            float input = GetModulatedFreq();
            if (bypass) return input;
            float smoothedInput = 0f;
            
            for(int i = 0; i < smoothLength; i++)
            {
                smoothedInput += buffer[smoothLength - 1 - i];
            }
            smoothedInput /= (float)smoothLength;
            buffer.RemoveAt(0);
            buffer.Add(input);
            return smoothedInput;
        }

        public override float GetPositiveValue()
        {
            return 0f;
        }
    }
}

