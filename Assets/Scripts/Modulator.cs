using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piafs
{
    public abstract class Modulator : MonoBehaviour
    {
        public float freq = 1.0f;
        public float amp = 1.0f;
        protected float phase = 0.0f;

        public List<Modulator> freqModulators;
        public List<Modulator> phaseModulators;
        public List<Modulator> ampModulators;

        public abstract float GetValue();
        public abstract float GetPositiveValue();

        protected float GetModulatedFreq()
        {
            float modulatedFreq = freq;

            foreach (Modulator freqModulator in freqModulators)
            {
                if(freqModulator != null) modulatedFreq += freqModulator.GetValue();
            }

            return modulatedFreq;
        }

        protected float GetModulatedPhase()
        {
            float modulatedPhase = phase;

            foreach (Modulator phaseModulator in phaseModulators)
            {
                modulatedPhase += phaseModulator.GetValue();
            }

            return modulatedPhase;
        }

        protected float GetModulatedAmp()
        {
            float modulatedAmp = amp;

            foreach (Modulator ampModulator in ampModulators)
            {
                modulatedAmp += ampModulator.GetPositiveValue();
            }

            return modulatedAmp;
        }

        public virtual bool Compare(Modulator other)
        {
            Type type = GetType();
            Type otherType = other.GetType();

            if (type != otherType)
            {
                return false;
            }

            if (freq != other.freq || amp != other.amp)
            {
                return false;
            }

            if (   freqModulators.Count != other.freqModulators.Count
                || ampModulators.Count != other.ampModulators.Count
                || phaseModulators.Count != other.phaseModulators.Count)
            {
                return false;
            }

            for (int i = 0 ; i < freqModulators.Count; ++i)
            {
                if (!freqModulators[i].Compare(other.freqModulators[i]))
                {
                    return false;
                }
            }

            for (int i = 0; i < ampModulators.Count; ++i)
            {
                if (!ampModulators[i].Compare(other.ampModulators[i]))
                {
                    return false;
                }
            }

            for (int i = 0; i < phaseModulators.Count; ++i)
            {
                if (!phaseModulators[i].Compare(other.phaseModulators[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public void GetDependenciesRecursive(List<Modulator> list)
        {
            //Debug.Log(name);
            if (!list.Contains(this)) list.Add(this);

            ampModulators.ForEach(a => a.GetDependenciesRecursive(list));
            freqModulators.ForEach(a => a.GetDependenciesRecursive(list));
            phaseModulators.ForEach(a => a.GetDependenciesRecursive(list));
        }
    }
}
