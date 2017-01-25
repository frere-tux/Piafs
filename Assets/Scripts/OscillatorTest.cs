using UnityEngine;
using System.Collections;

namespace Piafs
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    public class OscillatorTest : MonoBehaviour
    {
        public Modulator modulator;
        [Range(-1f,1f)]
        public float pan;

        public float debugScale = 0.01f;
        public float debugOffsetY = 5.0f;

        private float[] debugBuffer = new float[0];
        private int channelCount = 0;

        void OnEnable()
        {
            GetComponent<AudioSource>().Play();
        }

        void Update()
        {
            Toolkit.DrawCurve(debugBuffer, channelCount, debugScale, debugOffsetY);
        }


        void OnAudioFilterRead(float[] data, int channels)
        {
            if (modulator == null)
            {
                return;
            }
            float panRight = Mathf.InverseLerp(1f, -1f, pan);
            for (int n = 0; n < data.Length; n += channels)
            {
                float s = Mathf.Clamp(modulator.GetValue(), -1.0f, 1.0f);

                for (int i = 0; i < channels; i++)
                {
                    float panMultiplier = (i % 2 == 0)?panRight:1f-panRight;
                    data[n + i] = s * panMultiplier;
                }
            }

            debugBuffer = data;
            channelCount = channels;
        }
    }
}
