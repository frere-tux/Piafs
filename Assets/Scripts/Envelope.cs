using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Piafs
{
    [ExecuteInEditMode]
    [AddComponentMenu("Modulator/Envelope", -80)]
    public class Envelope : Modulator
    {
        [System.Serializable]
        public class EnvelopeCurve
        {
            public AnimationCurve curve;
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

        public AnimationCurve attack;
        public float attackDuration;
        public AnimationCurve decay;
        public float decayDuration;
        public AnimationCurve release;
        public float releaseDuration;
        public bool loop;
        public bool debugEnvelope;

        private int sampleTime;
        private int sampleRate;
        private bool triggered = false; 
        private float attackStrength = 0f;
        private float releaseStrength = 0f;
        private float currentValue = 0f;

        protected override void Awake()
        {
            base.Awake();
            sampleRate = AudioSettings.outputSampleRate;
            Debug.Log(gameObject.name);
            
        }

        void Update()
        {

            if(debugEnvelope)
            {
                if(Input.GetKeyDown(KeyCode.Space))
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
            attackStrength = currentValue * releaseStrength;
            triggered = true;
        }

        public override void Untrigger()
        {
            triggered = false;
            releaseStrength = Mathf.Lerp(attackStrength, 1f, currentValue);
            sampleTime = 0;
        }

        public float newGetValue()
        {
            sampleTime++;
            float time = sampleTime / (float)sampleRate;
            if (triggered)
            {
                float stopTime = env.GetReleaseTime();
                currentValue = attackStrength + (1f - env.GetAttackAmp()) * env.curve.Evaluate(env.loop ? Mathf.Min(stopTime, time):time);
                return currentValue;
            }
            else
            {
                currentValue = releaseStrength * env.GetReleaseAmp() * env.curve.Evaluate(time);
                return currentValue;
            }
        }

        public override float GetValue()
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
                    return Mathf.Lerp(attackStrength,1f, currentValue) * GetModulatedAmp();
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

        public override float GetPositiveValue()
        {
            return GetValue();
        }
    }

}
