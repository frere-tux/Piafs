using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    [RequireComponent(typeof(Collider2D))]
    public class Slot : MonoBehaviour
    {
        public List<Modulator> ampModulators, freqModulators, phaseModulators;
        public List<Modulator> ampOutputs, freqOutputs, phaseOutputs;

        private Modulator dropped;

        public void DropOscillator(Modulator _dropped)
        {
            dropped = _dropped;
            dropped.ampModulators.AddRange(ampModulators);
            dropped.freqModulators.AddRange(freqModulators);
            dropped.phaseModulators.AddRange(phaseModulators);

            foreach (Modulator m in ampOutputs) m.ampModulators.Add(dropped);
            foreach (Modulator m in freqOutputs) m.freqModulators.Add(dropped);
            foreach (Modulator m in phaseOutputs) m.phaseModulators.Add(dropped);
        }

        public void GrabOscillator()
        {
            foreach (Modulator m in ampModulators) dropped.ampModulators.Remove(m);
            foreach (Modulator m in freqModulators) dropped.freqModulators.Remove(m);
            foreach (Modulator m in phaseModulators) dropped.phaseModulators.Remove(m);

            foreach (Modulator m in ampOutputs) m.ampModulators.Remove (dropped);
            foreach (Modulator m in freqOutputs) m.freqModulators.Remove(dropped);
            foreach (Modulator m in phaseOutputs) m.phaseModulators.Remove(dropped);

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
