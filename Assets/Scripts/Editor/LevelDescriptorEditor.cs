using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Piafs
{
	[CustomEditor(typeof(LevelDescriptor))]
	public class LevelDescriptorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			LevelDescriptor levelDescriptor = ((LevelDescriptor)target);
			Color guiTint = new Color(1f, 1f, 1f);
			GUI.color = guiTint;
			if(levelDescriptor.librarySlotPrefab == null || 
				levelDescriptor.plugSlotPrefab == null || 
				levelDescriptor.interfaceHolder == null ||
				levelDescriptor.sliderPrefab == null)
			{
				EditorGUILayout.HelpBox("Level design helpers have not been filled.", MessageType.Warning);
			}
			if (levelDescriptor.levelManager == null)
			{
				EditorGUILayout.HelpBox("No level manager parent was found !", MessageType.Error);
				if(GUILayout.Button("Check again"))
				{
					if (levelDescriptor.transform.parent == null)
					{
						levelDescriptor.levelManager = null;
					}
					else
					{
						levelDescriptor.levelManager = levelDescriptor.transform.parent.gameObject.GetComponent<LevelManager>();
					}
				}
			}
			else if (levelDescriptor.outputModulator == null)
			{
				EditorGUILayout.HelpBox("No output modulator has been assigned !", MessageType.Error);
			}
			else if (!levelDescriptor.playerMixer.modulators.Contains(levelDescriptor.outputModulator))
			{
				EditorGUILayout.HelpBox("This level is not plugged to the player mixer.", MessageType.Warning);
				GUI.color = Color.yellow;
				if (GUILayout.Button("Plug to player mixer", GUILayout.MaxWidth(400f)))
				{

					levelDescriptor.playerMixer.modulators.Clear();
					levelDescriptor.playerMixer.modulators.Add(levelDescriptor.outputModulator);
				}
				GUI.color = guiTint;
			}
			else
			{
				if (GUILayout.Button(" < Refresh level elements > "))
				{
					levelDescriptor.RegisterLevelElements();
				}
				foreach(Trigger t in levelDescriptor.levelTriggers)
				{
					if(t != null)
					{
						if(GUILayout.RepeatButton("Activate "+t.name,GUILayout.MaxWidth(200f)))
						{
							if (!t.Triggered) t.Activate();
						}
						else
						{
							if (t.Triggered) t.Deactivate();
						}
					}
				}
				foreach (LevelControls l in levelDescriptor.levelControls)
				{
					if (l != null)
					{
						if(l is Slider)
						{
							Slider s = l as Slider;
							s.SetValueHard(EditorGUILayout.Slider("CONTROL : "+s.name,s.SteppedValue + s.min, s.min, s.max));
						}
						else if(l is Brick)
						{
							Brick b = l as Brick;
							GUILayout.BeginHorizontal();
							GUILayout.Label(b.name);
							for(int i = 0; i < levelDescriptor.levelSlots.Count; i++)
							{
								bool isInside = levelDescriptor.levelSlots[i] == b.slot;
								GUI.color = isInside ? Color.yellow : Color.green;
								if(GUILayout.Button(levelDescriptor.levelSlots[i].name))
								{
									Brick prev = levelDescriptor.levelSlots[i].SlottedBrick;
									if(prev != null) prev.Drop(prev.LibrarySlot);
									b.slot.GrabOscillator();
									levelDescriptor.levelSlots[i].GrabOscillator();
									if (!isInside)b.Drop(levelDescriptor.levelSlots[i]);
								}			
							}
							GUI.color = Color.white;
							GUILayout.EndHorizontal();
						}
					}
				}
			}
			EditorGUILayout.Space();
			GUI.color = Color.white;
			base.OnInspectorGUI();
		}
	}
}

