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

        public bool valid;

		[SerializeField]
        private Brick slottedBrick;
        public Brick SlottedBrick
        {
            get { return slottedBrick; }
        }

        public void DropBrick(Brick _brick)
        {
			Debug.Log("drop brick");
            slottedBrick = _brick;
            Modulator dropped = slottedBrick.modulator;

            dropped.ampModulators.AddRange(ampModulators);
            dropped.freqModulators.AddRange(freqModulators);
            dropped.phaseModulators.AddRange(phaseModulators);

            foreach (Modulator m in ampOutputs) m.ampModulators.Add(dropped);
            foreach (Modulator m in freqOutputs) m.freqModulators.Add(dropped);
            foreach (Modulator m in phaseOutputs) m.phaseModulators.Add(dropped);
        }

		public void SetSlottedBrick(Brick _brick)
		{
			slottedBrick = _brick;
		}

        public void GrabOscillator()
        {
            if(slottedBrick != null)
            {
                Modulator dropped = slottedBrick.modulator;

                foreach (Modulator m in ampModulators) dropped.ampModulators.Remove(m);
                foreach (Modulator m in freqModulators) dropped.freqModulators.Remove(m);
                foreach (Modulator m in phaseModulators) dropped.phaseModulators.Remove(m);

                foreach (Modulator m in ampOutputs) m.ampModulators.Remove(dropped);
                foreach (Modulator m in freqOutputs) m.freqModulators.Remove(dropped);
                foreach (Modulator m in phaseOutputs) m.phaseModulators.Remove(dropped);

				slottedBrick.slot = slottedBrick.librarySlot;
                slottedBrick = null;
            }
        }

        public void OnMouseEnter()
        {
            InputManager.hoveredSlot = this;
        }

        public void OnMouseExit()
        {
            InputManager.hoveredSlot = null;
        }

        public void GetDependencies(List<Modulator> result)
        {
            ampModulators.ForEach(a => a.GetDependenciesRecursive(result));
            freqModulators.ForEach(a => a.GetDependenciesRecursive(result));
            phaseModulators.ForEach(a => a.GetDependenciesRecursive(result));
            ampOutputs.ForEach(a => a.GetDependenciesRecursive(result));
            freqOutputs.ForEach(a => a.GetDependenciesRecursive(result));
            phaseOutputs.ForEach(a => a.GetDependenciesRecursive(result));
        }

		public override string ToString()
		{
			return name;
		}
	}

}
