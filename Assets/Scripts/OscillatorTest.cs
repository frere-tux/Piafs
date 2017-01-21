using UnityEngine;
using System.Collections;

namespace Piafs
{
    [RequireComponent(typeof(AudioSource))]
    public class OscillatorTest : MonoBehaviour
    {
        public Oscillator osc;

        public float debugScale = 0.01f;
        public float debugOffsetY = 1.0f;

        private float[] debugBuffer = new float[0];
        private int channelCount = 0;

        void Update()
        {
            Toolkit.DrawCurve(debugBuffer, channelCount, debugScale, debugOffsetY);
        }


        void OnAudioFilterRead(float[] data, int channels)
        {
            if (osc == null)
            {
                return;
            }

            for (int n = 0; n < data.Length; n += channels)
            {
                float s = Mathf.Clamp(osc.GetNextSample(), -1.0f, 1.0f);

                for (int i = 0; i < channels; i++)
                {
                    data[n + i] = s;
                }
            }

            debugBuffer = data;
            channelCount = channels;
        }
    }
}
