using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public class AnimBirdCarton : MonoBehaviour
    {
        public SpriteRenderer stick;
        public Modulator modulator;
        public float rotationStickMax;
        public float breathFrequency;
        public float breathRotationMax;
        public float moveDistance;
        public float moveSpeed;
        public bool move = false;

        private Vector3 baseStickRotation;
        private Vector3 baseStickPosition;

        private float smoothFreq = 0.0f;

        void Start()
        {
            baseStickRotation = stick.transform.localEulerAngles;
            baseStickPosition = stick.transform.localPosition;
        }

        void Update()
        {
            Vector3 position = stick.transform.localPosition;
            Vector3 fromBaseToPosition = position - baseStickPosition;
            if (move)
            {
                if (fromBaseToPosition.sqrMagnitude < moveDistance*moveDistance)
                {
                    if (fromBaseToPosition == Vector3.zero)
                    {
                        position += stick.transform.up * Time.deltaTime * moveSpeed;
                    }
                    else
                    {
                        fromBaseToPosition.Normalize();
                        position += fromBaseToPosition * Time.deltaTime * moveSpeed;
                    }


                    stick.transform.localPosition = position;
                }
            }
            else
            {
                if (fromBaseToPosition.sqrMagnitude > 0.01)
                {
                    fromBaseToPosition.Normalize();
                    position -= fromBaseToPosition * Time.deltaTime * moveSpeed;

                    stick.transform.localPosition = position;
                }
                else
                {
                    stick.transform.localPosition = baseStickPosition;
                }
            }



            float amp = modulator.GetModulatedAmp(true);
            amp = Mathf.Clamp01(amp);

            float freq = modulator.GetModulatedFreq();
            freq = Mathf.Log(freq / 20.0f, 2.0f) * 4.0f;
            freq = (freq * 2.0f) - 1.0f;
            freq *= Mathf.Pow(amp, 0.5f);

            float breathRotation = Mathf.Sin(Time.time * breathFrequency);
            breathRotation *= breathRotationMax;

            float freqRotation = Mathf.Sin(Time.time * freq);
            freqRotation *= amp * rotationStickMax;

            Vector3 stickRotation = baseStickRotation;
            stickRotation.z += breathRotation + freqRotation;

            stick.transform.localEulerAngles = stickRotation;
        }
    }
}
