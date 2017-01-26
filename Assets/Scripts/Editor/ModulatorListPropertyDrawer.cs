using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Piafs
{
	[CustomEditor(typeof(Modulator))]
	public class ModulatorEditor : Editor
	{
		private bool selectNewModulator = false;
		private int modulatorImpact = 0;
		private Modulator mod;

		public override void OnInspectorGUI()
		{
			mod = (Modulator)target;
			GUILayout.BeginVertical();
			if (GUILayout.Button("--> Put into gameplay brick", GUILayout.MaxWidth(200f)))
			{
				PutIntoGameplayBrick(mod);
			}
			if (selectNewModulator == false)
			{
				if (GUILayout.Button("Add freq modulator", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = true;
					modulatorImpact = 0;
				}
				if (GUILayout.Button("Add amp modulator", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = true;
					modulatorImpact = 1;
				}
				if (GUILayout.Button("Add phase modulator", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = true;
					modulatorImpact = 2;
				}
				GUI.color = Color.yellow;
				if (GUILayout.Button("Clean empty modulators", GUILayout.MaxWidth(200f)))
				{
					mod.CleanLists();
				}
				GUI.color = Color.white;
			}
			else
			{
				if (GUILayout.Button("Sine", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<SinOscillator>());
				}
				if (GUILayout.Button("Square", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<SquareOscillator>());
				}
				if (GUILayout.Button("Curve", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<CurveOscillator>());
				}
				if (GUILayout.Button("Noise", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<WhiteNoise>());
				}
				if (GUILayout.Button("Envelope", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<EnvelopeModulator>());
				}
				if (GUILayout.Button("Fixed", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<FixedModulator>());
				}
				if (GUILayout.Button("Random", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<RandomFixedModulator>());
				}
				if (GUILayout.Button("Filter", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<FilterModulator>());
				}
				if (GUILayout.Button("Multiplier", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<MultiplyModulator>());
				}
				if (GUILayout.Button("Mixer", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					GetModulatorList().Add(NewModulator<Mixer>());
				}
				GUI.color = Color.red;
				if (GUILayout.Button("<- BACK", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
				}
				GUI.color = Color.white;
			}
			
			GUILayout.EndVertical();
			base.OnInspectorGUI();
		}

		private string NewObjPrefix()
		{
			switch(modulatorImpact)
			{
				case 0:
					return "Freq";
				case 1:
					return "Amp";
				case 2:
					return "Phase";
			}
			return "";
		}

		private Modulator NewModulator<T>() where T : Modulator
		{
			GameObject g = new GameObject();
			T nuMod = g.AddComponent<T>();
			g.name = nuMod.GetType().Name + " --> " + NewObjPrefix();
			g.transform.parent = mod.gameObject.transform;
			Selection.activeGameObject = g;
			return nuMod;
		}

		public List<Modulator> GetModulatorList()
		{
			switch(modulatorImpact)
			{
				case 0: return mod.freqModulators;
				case 1: return mod.ampModulators;
				case 2: return mod.phaseModulators;
			}
			return null;
		}

		public void PutIntoGameplayBrick(Modulator thisModulator)
		{
			GameObject g = thisModulator.gameObject;
			while(g.GetComponent<LevelDescriptor>() == null)
			{
				if(g.transform.parent != null)
				{
					g = g.transform.parent.gameObject;
				}
				else
				{
					throw new System.Exception("This modulator doesn't belong to a level !");
				}
			}
			Modulator[] allMods = g.GetComponentsInChildren<Modulator>();
			List<Modulator> ampModulated = new List<Modulator>();
			List<Modulator> freqModulated = new List<Modulator>();
			List<Modulator> phaseModulated = new List<Modulator>();
			foreach(Modulator m in allMods)
			{
				if(m.ampModulators.Contains(thisModulator))
				{
					ampModulated.Add(m);
				}
				if (m.freqModulators.Contains(thisModulator))
				{
					freqModulated.Add(m);
				}
				if (m.phaseModulators.Contains(thisModulator))
				{
					phaseModulated.Add(m);
				}
			}
			GameObject librarySlot = Instantiate(g.GetComponent<LevelDescriptor>().librarySlotPrefab);
			Brick newBrick = librarySlot.GetComponentInChildren<Brick>();
			Slot newPlugSlot = Instantiate(g.GetComponent<LevelDescriptor>().plugSlotPrefab).GetComponentInChildren<Slot>();
			newBrick.modulator = thisModulator;
			newBrick.name = "Brick :"+ thisModulator.name;
			newBrick.rightSlot = newPlugSlot;
			newBrick.librarySlot = librarySlot.GetComponent<Slot>();
			//newBrick.Drop(newBrick.librarySlot);
			
			newPlugSlot.ampModulators = thisModulator.ampModulators;
			newPlugSlot.ampModulators = thisModulator.freqModulators;
			newPlugSlot.ampModulators = thisModulator.phaseModulators;
			newPlugSlot.ampOutputs = ampModulated;
			newPlugSlot.freqOutputs = freqModulated;
			newPlugSlot.phaseOutputs = phaseModulated;

		}
	}

	[CustomEditor(typeof(SinOscillator))]
	public class SinOscillatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(SquareOscillator))]
	public class SquareOscillatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(CurveOscillator))]
	public class CurveOscillatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(WhiteNoise))]
	public class WhiteNoiseEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(EnvelopeModulator))]
	public class EnvelopeModulatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(FixedModulator))]
	public class FixedModulatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(MultiplyModulator))]
	public class MultiplyModulatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(RandomFixedModulator))]
	public class RandomFixedModulatorEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}

	[CustomEditor(typeof(Mixer))]
	public class MixerEditor : ModulatorEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}
}

