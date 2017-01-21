using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Piafs
{
    public class Envelope : Modulator
    {
        public AnimationCurve attack;
        public float attackDuration;
        public AnimationCurve decay;
        public float decayDuration;
        public AnimationCurve release;
        public float releaseDuration;
        public float amp;
        public bool loop;
        public bool debugEnvelope;

        private int sampleTime;
        private int sampleRate;
        private bool triggered = false;
        private float attackAmplitude = 0f;
        private float releaseAmplitude = 0f;
        private float currentValue = 0f;

        void Start()
        {
            sampleRate = AudioSettings.outputSampleRate;
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

        public void Trigger()
        {
            sampleTime = 0;
            attackAmplitude = currentValue * releaseAmplitude;
            triggered = true;
        }

        public void Untrigger()
        {
            triggered = false;
            releaseAmplitude = Mathf.Lerp(attackAmplitude, 1f, currentValue);
            sampleTime = 0;
        }

        public override float GetValue()
        {
            
            sampleTime++;
            if (triggered)
            {
                if (!loop && sampleTime >= (attackDuration + releaseDuration + decayDuration) * sampleRate)
                {
                    Untrigger();
                }
                if (sampleTime < attackDuration * sampleRate)
                {
                    currentValue = attack.Evaluate((sampleTime / (float)sampleRate) / attackDuration);
                    return Mathf.Lerp(attackAmplitude,1f, currentValue) * amp;
                }
                else
                {
                    currentValue = decay.Evaluate((sampleTime / (float)sampleRate - attackDuration) / decayDuration);
                    return Mathf.Lerp(attackAmplitude, 1f, currentValue) * amp;
                }
            }
            else
            {
                if (sampleTime < (releaseDuration) * sampleRate)
                {
                    currentValue = release.Evaluate((sampleTime / (float)sampleRate) / releaseDuration);
                    return currentValue * releaseAmplitude * amp;
                }
                else
                {
                    currentValue = 0f;
                    return currentValue * amp;
                }
            }
        }

        public override float GetPositiveValue()
        {
            return GetValue();
        }
    }

}
