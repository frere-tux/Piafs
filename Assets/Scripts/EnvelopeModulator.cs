using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Piafs
{
    [ExecuteInEditMode]
    [AddComponentMenu("Modulator/EnvelopeModulator", -80)]
    public class EnvelopeModulator : Modulator
    {
        [System.Serializable]
        public class EnvelopeCurve
        {
			public AnimationCurve curve = new AnimationCurve(
				new Keyframe(0f, 0f, 0f, 0f),
				new Keyframe(1f, 1f, 0f, 0f),
				new Keyframe(2f, 0f, 0f, 0f)
				);
            public int attackPoint;
            public int releasePoint;
            public bool loop;

            public float GetAttackTime()
            {
                return curve[attackPoint].time;
            }

            public float GetAttackAmp()
            {
                return curve[attackPoint].value;
            }

            public float GetReleaseTime()
            {
                return curve[curve.length - 1 + releasePoint].time;
            }

            public float GetReleaseAmp()
            {
                return curve[curve.length - 1 + releasePoint].value;
            }
        }

        public EnvelopeCurve env;
        public bool debugEnvelope = true;

        private int sampleTime;
        private float time;
        private int sampleRate;
        private bool triggered = false;
        private float attackStrength = 0f;
        private float releaseStrength = 0f;
        private float currentValue = 0f;

        protected override void Awake()
        {
            base.Awake();
            sampleRate = AudioSettings.outputSampleRate;

        }

        void Update()
        {

            if (debugEnvelope)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Trigger();
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Untrigger();
                }
            }
        }

        public override void Trigger()
        {
            sampleTime = 0;
            attackStrength = currentValue;
            triggered = true;
        }

        public override void Untrigger()
        {
            triggered = false;
            releaseStrength = currentValue;
            sampleTime = Mathf.RoundToInt(env.GetReleaseTime() * sampleRate);
        }

        public override float GetValue()
        {
            sampleTime++;
            time = sampleTime / (float)sampleRate;
            if (triggered)
            {
                float stopTime = env.GetReleaseTime();
                currentValue = attackStrength + (1f - attackStrength) * env.curve.Evaluate(env.loop ? Mathf.Min(stopTime, time) : time);
                return currentValue * GetModulatedAmp();
            }
            else
            {
				float releaseAmp = env.GetReleaseAmp();
				if (releaseAmp == 0f) releaseAmp = 1f;
                currentValue = releaseStrength * env.curve.Evaluate(time) / releaseAmp;
                return currentValue * GetModulatedAmp();
            }
        }
        /*
        public float OldGetValue()
        {

            sampleTime++;
            if (triggered)
            {
                if (!loop && sampleTime >= (attackDuration + decayDuration) * sampleRate)
                {
                    Untrigger();
                    return releaseStrength;
                }
                if (sampleTime < attackDuration * sampleRate)
                {
                    currentValue = attack.Evaluate((sampleTime / (float)sampleRate) / attackDuration);
                    return Mathf.Lerp(attackStrength, 1f, currentValue) * GetModulatedAmp();
                }
                else
                {
                    currentValue = decay.Evaluate((sampleTime / (float)sampleRate - attackDuration) / decayDuration);
                    return Mathf.Lerp(attackStrength, 1f, currentValue) * GetModulatedAmp();
                }
            }
            else
            {
                if (sampleTime < (releaseDuration) * sampleRate)
                {
                    currentValue = release.Evaluate((sampleTime / (float)sampleRate) / releaseDuration);
                    return currentValue * releaseStrength * GetModulatedAmp();
                }
                else
                {
                    currentValue = 0f;
                    return currentValue * GetModulatedAmp();
                }
            }
        }
        */

        public override float GetPositiveValue()
        {
            return GetValue();
        }
    }

}
