using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    [ExecuteInEditMode]
    [AddComponentMenu("Modulator/Fixed Modulator", -80)]
    public class FixedModulator : Modulator
    {
        private float smoothing = 0f;
        private float rawAmp;
        private float smoothThreshold = 1f;

        protected override void Awake()
        {
            base.Awake();
            rawAmp = amp;
        }

        public void SetSmoothing(float _amount, float _threshold)
        {
            smoothing = _amount;
            smoothThreshold = _threshold;
        }

        public void SetValueSmooth(float _val)
        {
            rawAmp = _val;
        }

        public void JumpToRawValue()
        {
            amp = rawAmp;
        }

        public override float GetValue()
        {
            if (smoothing > 0f)
            {
                amp = Mathf.Lerp(amp, rawAmp, smoothing);
                if (Mathf.Abs(amp - rawAmp) < smoothThreshold)
                {
                    amp = rawAmp;
                }
            }
            return GetModulatedAmp();
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }
    }
}
