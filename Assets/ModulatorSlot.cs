using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    [RequireComponent(typeof(Collider2D))]
    public class ModulatorSlot : MonoBehaviour
    {
        public List<Modulator> ampModulators, freqModulators, phaseModulators;
        private Oscillator dropped;

        public void DropOscillator(Oscillator _dropped)
        {
            dropped = _dropped;
            dropped.ampModulators.AddRange(ampModulators);
            dropped.freqModulators.AddRange(freqModulators);
            dropped.phaseModulators.AddRange(phaseModulators);
        }

        public void GrabOscillator()
        {
            foreach(Modulator m in ampModulators)
            {
                dropped.ampModulators.Remove(m);
            }
            foreach (Modulator m in freqModulators)
            {
                dropped.freqModulators.Remove(m);
            }
            foreach (Modulator m in phaseModulators)
            {
                dropped.phaseModulators.Remove(m);
            }
            dropped = null;
        }

        public void OnMouseEnter()
        {
            InputManager.hoveredSlot = this;
        }

        public void OnMouseExit()
        {
            InputManager.hoveredSlot = null;
        }
    }

}
