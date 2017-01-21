using UnityEngine;
using System.Collections;

namespace Piafs
{
    [RequireComponent(typeof(AudioSource))]
    public class OscillatorTest : MonoBehaviour
    {
        public CurveOscillator curveOsc = new CurveOscillator();
        public CurveOscillator freqModulatorOsc1 = new CurveOscillator();
        public CurveOscillator freqModulatorOsc2 = new CurveOscillator();

        void Start()
        {
            curveOsc.Init();
            freqModulatorOsc1.Init();
            freqModulatorOsc2.Init();
        }


        void OnAudioFilterRead(float[] data, int channels)
        {
            for (int n = 0; n < data.Length; n += channels)
            {
                freqModulatorOsc1.freqModulation = freqModulatorOsc2.GetNextSample();
                curveOsc.freqModulation = freqModulatorOsc1.GetNextSample();

                float s = Mathf.Clamp(curveOsc.GetNextSample(), -1.0f, 1.0f);

                for (int i = 0; i < channels; i++)
                {
                    data[n + i] = s;
                }
            }
        }
    }
}
