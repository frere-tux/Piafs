using UnityEngine;
using System.Collections;

namespace Piafs
{
    [RequireComponent(typeof(AudioSource))]
    public class OscillatorTest : MonoBehaviour
    {
        public SinOscillator sinOsc = new SinOscillator();
        public SquareOscilator squareOsc = new SquareOscilator();

        void Start()
        {
            sinOsc.Init();
            squareOsc.Init();
        }


        void OnAudioFilterRead(float[] data, int channels)
        {

            for (int n = 0; n < data.Length; n += channels)
            {
                float s1 = sinOsc.GetNextSample();
                float s2 = squareOsc.GetNextSample();

                for (int i = 0; i < channels; i++)
                {
                    if (i % channels == 0)
                        data[n + i] = s1;
                    else
                        data[n + i] = s2;
                }
            }
        }
    }
}
