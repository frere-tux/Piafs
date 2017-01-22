using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public class AnimBird : MonoBehaviour
    {
        public SpriteRenderer eye;
        public SpriteRenderer eyebrow;
        public SpriteRenderer beak;
        public SpriteRenderer wing;
        public SpriteRenderer tail;
        public SpriteRenderer legs;
        public SpriteRenderer body;
        public SpriteRenderer head;

        [Header("EyeBlink")]
        public float blinkTimeMin = 0.1f;
        public float blinkTimeMax = 5.0f;

        [Header("Frown")]
        public float frownTimeMin = 10.0f;
        public float frownTimeMax = 15.0f;

        [Header("WingFlap")]
        public float wingFlapTimeMin = 15.0f;
        public float wingFlapTimeMax = 30.0f;
        public AnimationCurve wingCurve;

        [Header("TailFlap")]
        public float tailFlapTimeMin = 10.0f;
        public float tailFlapTimeMax = 20.0f;
        public AnimationCurve tailCurve;

        [Header("Singing")]
        public float beakRotationMax = 50.0f;
        public float headRotationMax = 50.0f;
        public float bodyRotationMax = 15.0f;
        public float headRotationSmoothing = 0.999f;
        public Modulator modulator;

        [Header("Breath")]
        public float breathFrequency = 3.0f;
        public float breathRotationMax = 10.0f;

        private Vector3 baseBeakRotation;
        private Vector3 baseHeadRotation;
        private Vector3 baseBodyRotation;
        private Vector3 baseTailRotation;
        private float smoothFreq = 0.0f;

        void Start()
        {
            eye.enabled = false;
            eyebrow.enabled = false;
            beak.enabled = true;
            wing.enabled = true;
            tail.enabled = true;
            legs.enabled = true;
            body.enabled = true;

            baseBeakRotation = beak.transform.localEulerAngles;
            baseHeadRotation = head.transform.localEulerAngles;
            baseBodyRotation = body.transform.localEulerAngles;
            baseTailRotation = tail.transform.localEulerAngles;

            StartCoroutine(EyeBlink());
            StartCoroutine(Frown());
            StartCoroutine(FlapWing());
            StartCoroutine(FlapTail());
        }

        void Update()
        {
            // Amplitude to Beak
            float amp = modulator.GetModulatedAmp(true);
            amp = Mathf.Clamp01(amp);


            float ampRotation = amp *beakRotationMax;
            ampRotation = Mathf.Min(ampRotation, beakRotationMax);

            Vector3 beakRotation = baseBeakRotation;
            beakRotation.z -= ampRotation;

            beak.transform.localEulerAngles = beakRotation;


            // Amplitude to Head, Body and tail
            float freq = modulator.GetModulatedFreq();

            freq = Mathf.Log(freq / 20.0f, 2.0f) * 0.1f;

            freq = (freq * 2.0f) - 1.0f;

            freq *=  Mathf.Pow(amp, 0.5f);

            smoothFreq = Toolkit.Damp(smoothFreq, freq, headRotationSmoothing, Time.deltaTime);

            Vector3 headRotation = baseHeadRotation;
            headRotation.z += smoothFreq * headRotationMax;

            head.transform.localEulerAngles = headRotation;

            Vector3 tailRotation = baseTailRotation;
            tailRotation.z -= smoothFreq * bodyRotationMax * 8.0f;

            tail.transform.localEulerAngles = tailRotation;

            Vector3 bodyRotation = baseBodyRotation;
            bodyRotation.z -= smoothFreq * bodyRotationMax;

            // Breath
            float breath = Mathf.Sin(Time.time * breathFrequency);
            bodyRotation.z += breath * breathRotationMax;

            body.transform.localEulerAngles = bodyRotation;
        }

        IEnumerator EyeBlink()
        {
            while (true)
            {
                float waitTime = UnityEngine.Random.Range(blinkTimeMin, blinkTimeMax);
                yield return new WaitForSeconds(waitTime);

                eye.enabled = true;

                waitTime = UnityEngine.Random.Range(0.02f, 0.10f);
                yield return new WaitForSeconds(0.1f);

                eye.enabled = false;

                if (UnityEngine.Random.value > 0.5f)
                {
                    waitTime = UnityEngine.Random.Range(0.1f, 0.2f);
                    yield return new WaitForSeconds(waitTime);

                    eye.enabled = true;

                    waitTime = UnityEngine.Random.Range(0.02f, 0.10f);
                    yield return new WaitForSeconds(0.1f);

                    eye.enabled = false;
                }
            }
        }

        IEnumerator Frown()
        {
            while (true)
            {
                float waitTime = UnityEngine.Random.Range(frownTimeMin, frownTimeMax);
                yield return new WaitForSeconds(waitTime);

                eyebrow.enabled = true;

                waitTime = UnityEngine.Random.Range(frownTimeMin*0.5f, frownTimeMax*0.5f);
                yield return new WaitForSeconds(waitTime);

                eyebrow.enabled = false;
            }
        }

        IEnumerator FlapWing()
        {
            Vector3 baseRotation = wing.transform.localEulerAngles;

            while (true)
            {
                float waitTime = UnityEngine.Random.Range(wingFlapTimeMin, wingFlapTimeMax);
                yield return new WaitForSeconds(waitTime);

                float startTime = Time.time;

                bool done = false;
                while (!done)
                {
                    float time = Time.time;
                    float duration = time - startTime;

                    if (duration < wingCurve.keys.Last().time)
                    {
                        float zRotation = wingCurve.Evaluate(duration);
                        Vector3 rotation = baseRotation;
                        rotation.z += zRotation;

                        wing.transform.localEulerAngles = rotation;

                        yield return null;
                    }
                    else
                    {
                        done = true;
                    }
                }

                wing.transform.localEulerAngles = baseRotation;
            }
        }

        IEnumerator FlapTail()
        {
            Vector3 baseRotation = tail.transform.localEulerAngles;

            while (true)
            {
                float waitTime = UnityEngine.Random.Range(tailFlapTimeMin, tailFlapTimeMax);
                yield return new WaitForSeconds(waitTime);

                float startTime = Time.time;

                bool done = false;
                while (!done)
                {
                    float time = Time.time;
                    float duration = time - startTime;

                    if (duration < tailCurve.keys.Last().time)
                    {
                        float zRotation = tailCurve.Evaluate(duration);
                        Vector3 rotation = baseRotation;
                        rotation.z += zRotation;

                        tail.transform.localEulerAngles = rotation;

                        yield return null;
                    }
                    else
                    {
                        done = true;
                    }
                }

                tail.transform.localEulerAngles = baseRotation;
            }
        }
    }
}
