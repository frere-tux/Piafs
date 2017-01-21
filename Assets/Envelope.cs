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
        public bool loop;

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

        public void Trigger()
        {
            sampleTime = 0;
            attackAmplitude = currentValue;
            triggered = true;
        }

        public void Untrigger()
        {
            triggered = false;
            releaseAmplitude = currentValue;
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
                    return currentValue;
                }
                else
                {
                    currentValue = decay.Evaluate((sampleTime / (float)sampleRate - attackDuration) / decayDuration);
                    return currentValue;
                }
            }
            else
            {
                if (sampleTime < (releaseDuration) * sampleRate)
                {
                    currentValue = release.Evaluate((sampleTime / (float)sampleRate) / releaseDuration);
                    return currentValue * releaseAmplitude;
                }
                else
                {
                    currentValue = 0f;
                    return currentValue;
                }
            }
        }
    }

}
