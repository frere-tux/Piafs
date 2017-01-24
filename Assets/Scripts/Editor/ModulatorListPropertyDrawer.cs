using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Piafs
{
	[CustomEditor(typeof(Modulator))]
	public class ModulatorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			Modulator mod = (Modulator)target;
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Add freq. mod.", GUILayout.MaxWidth(200f)))
			{
				
			}
			GUILayout.Button("New amplitude modulator", GUILayout.MaxWidth(200f));
			GUILayout.Button("New phase modulator", GUILayout.MaxWidth(200f));
			GUILayout.EndHorizontal();
			base.OnInspectorGUI();
		}

		public Modulator NewModulator<T>() where T : Modulator
		{
			GameObject g = new GameObject();
			T mod = g.AddComponent<T>();
			g.name = mod.GetType().Name;
			Selection.activeGameObject = g;
			return mod;
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
}

