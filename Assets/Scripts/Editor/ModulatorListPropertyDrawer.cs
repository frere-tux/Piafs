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
		protected Modulator mod;
		protected LevelDescriptor levelDescriptor;

		public override void OnInspectorGUI()
		{
			ModulatorHelpers();
			DefaultInspectorGUI();
		}

		protected void ModulatorHelpers()
		{
			mod = (Modulator)target;

			GUILayout.BeginVertical();
			if (selectNewModulator == false)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("+ Freq Mod", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = true;
					modulatorImpact = 0;
				}
				if (GUILayout.Button("+ Amp Mod", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = true;
					modulatorImpact = 1;
				}
				if (GUILayout.Button("+ Phase Mod", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = true;
					modulatorImpact = 2;
				}
				GUI.color = Color.yellow;
				/*if (GUILayout.Button("CLEAN", GUILayout.MaxWidth(200f)))
				{
					mod.CleanLists();
				}*/
				GUILayout.EndHorizontal();
				GUI.color = Color.white;
			}
			else
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Sin", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<SinOscillator>());
				}
				if (GUILayout.Button("Sqr", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<SquareOscillator>());
				}
				if (GUILayout.Button("Curve", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<CurveOscillator>());
				}
				if (GUILayout.Button("Noise", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<WhiteNoise>());
				}
				if (GUILayout.Button("Env", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<EnvelopeModulator>());
				}
				if (GUILayout.Button("Fixed", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<FixedModulator>());
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Rnd", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<RandomFixedModulator>());
				}
				if (GUILayout.Button("LPF", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<FilterModulator>());
				}
				if (GUILayout.Button("Mult", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<MultiplyModulator>());
				}
				if (GUILayout.Button("Mixer", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
					AddModulatorToList(NewModulator<Mixer>());
				}
				GUI.color = Color.red;
				if (GUILayout.Button("<- BACK", GUILayout.MaxWidth(200f)))
				{
					selectNewModulator = false;
				}
				GUILayout.EndHorizontal();
				GUI.color = Color.white;
			}
			GUI.color = Color.cyan;
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("--> Put into gameplay brick", GUILayout.MaxWidth(200f)))
			{
				PutIntoGameplayBrick(mod);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUI.color = Color.white;
			GUILayout.EndVertical();
		}

		protected void DefaultInspectorGUI()
		{
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
			Undo.RegisterCreatedObjectUndo(g, "Create new modulator");
			return nuMod;
		}

		public List<Modulator> AddModulatorToList(Modulator newMod)
		{
			string listName = "";
			switch(modulatorImpact)
			{
				case 0:
					listName = "freqModulators";
					break;
				case 1:
					listName = "ampModulators";
					break;
				case 2:
					listName = "phaseModulators";
					break;
			}
			SerializedProperty property = serializedObject.FindProperty(listName);
			property.arraySize++;
			property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue = newMod;
			serializedObject.ApplyModifiedProperties();
			return null;
		}

		public void PutIntoGameplayBrick(Modulator thisModulator)
		{
			GameObject g = GetLevelObject(thisModulator);
			Modulator[] allMods = g.GetComponentsInChildren<Modulator>();
			List<Modulator> ampModulated = new List<Modulator>();
			List<Modulator> freqModulated = new List<Modulator>();
			List<Modulator> phaseModulated = new List<Modulator>();
			foreach(Modulator m in allMods)
			{
				if(m.ampModulators.Contains(thisModulator))
				{
					Undo.RecordObject(m, "Unplug Modulated");
					ampModulated.Add(m);
					m.ampModulators.Remove(thisModulator);
				}
				if (m.freqModulators.Contains(thisModulator))
				{
					Undo.RecordObject(m, "Unplug Modulated");
					freqModulated.Add(m);
					m.freqModulators.Remove(thisModulator);
				}
				if (m.phaseModulators.Contains(thisModulator))
				{
					Undo.RecordObject(m, "Unplug Modulated");
					phaseModulated.Add(m);
					m.phaseModulators.Remove(thisModulator);
				}
			}
			LevelDescriptor level = g.GetComponent<LevelDescriptor>();
			GameObject librarySlot = Instantiate(level.librarySlotPrefab);
			librarySlot.transform.SetParent(level.interfaceHolder.transform);
			Brick newBrick = librarySlot.GetComponentInChildren<Brick>();
			librarySlot.name = "+ New Library Slot";

			newBrick.modulator = thisModulator;
			newBrick.name = "Brick :"+ thisModulator.name;
			newBrick.rightSlot = newBrick.librarySlot;
			newBrick.librarySlot = librarySlot.GetComponent<Slot>();

			Undo.RegisterCreatedObjectUndo(librarySlot, "Create New Library Slot");
			Selection.objects = new Object[] { librarySlot };

			bool brickIsConnected =
				thisModulator.ampModulators.Count > 0 ||
				thisModulator.freqModulators.Count > 0 ||
				thisModulator.phaseModulators.Count > 0 ||
				ampModulated.Count > 0 ||
				freqModulated.Count > 0 ||
				phaseModulated.Count > 0;

			if (brickIsConnected)
			{
				Slot newPlugSlot = Instantiate(level.plugSlotPrefab).GetComponentInChildren<Slot>();
				newPlugSlot.transform.SetParent(level.interfaceHolder.transform);
				newPlugSlot.name = "+ New Plug Slot";
				newBrick.rightSlot = newPlugSlot;

				newPlugSlot.ampModulators = new List<Modulator>(thisModulator.ampModulators);
				newPlugSlot.freqModulators = new List<Modulator>(thisModulator.freqModulators);
				newPlugSlot.phaseModulators = new List<Modulator>(thisModulator.phaseModulators);

				Undo.RecordObject(thisModulator, "Unplug Brick Modulator");
				newPlugSlot.ampModulators.ForEach(a => thisModulator.ampModulators.Remove(a));
				newPlugSlot.freqModulators.ForEach(a => thisModulator.freqModulators.Remove(a));
				newPlugSlot.phaseModulators.ForEach(a => thisModulator.phaseModulators.Remove(a));

				newPlugSlot.ampOutputs = ampModulated;
				newPlugSlot.freqOutputs = freqModulated;
				newPlugSlot.phaseOutputs = phaseModulated;

				Undo.RegisterCreatedObjectUndo(newPlugSlot.gameObject, "Create New Brick & Plug Slot");
				Selection.objects = new Object[] { librarySlot, newPlugSlot.gameObject };
			}
		}

		protected GameObject GetLevelObject(Modulator _modulator)
		{
			GameObject g = _modulator.gameObject;
			while (g.GetComponent<LevelDescriptor>() == null)
			{
				if (g.transform.parent != null)
				{
					g = g.transform.parent.gameObject;
				}
				else
				{
					throw new System.Exception("This modulator doesn't belong to a level !");
				}
			}
			return g;
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
			ModulatorHelpers();
			mod = (Modulator)target;
			levelDescriptor = GetLevelObject(mod).GetComponent<LevelDescriptor>();
			GUI.color = Color.cyan;
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("--> Control with new slider", GUILayout.MaxWidth(200f)))
			{
				PutIntoSlider((FixedModulator)mod);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUI.color = Color.white;
			DefaultInspectorGUI();
		}

		private void PutIntoSlider(FixedModulator fixMod)
		{
			GameObject s = Instantiate(levelDescriptor.sliderPrefab);
			s.transform.SetParent(levelDescriptor.interfaceHolder.transform);
			s.name = "+ New Slider";
			Slider slider = s.GetComponentInChildren<Slider>();
			slider.fixedModulators.Add(fixMod);
			slider.rightValue = fixMod.amp;
			slider.min = fixMod.amp * 0.5f;
			slider.max = fixMod.amp!= 0f ? fixMod.amp * 1.5f : 1f;
			slider.step = fixMod.amp != 0f ? fixMod.amp * 0.5f : 1f;
			Undo.RegisterCreatedObjectUndo(s, "Create Slider From Fixed Modulator");
			Selection.activeGameObject = slider.gameObject;
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

