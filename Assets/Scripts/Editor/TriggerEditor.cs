using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Piafs
{
	[CustomEditor(typeof(Trigger))]
	public class TriggerEditor : Editor
	{
		private bool triggered; 

		public override void OnInspectorGUI()
		{
			GUI.color = Color.green;
			if(GUILayout.RepeatButton("Manual trigger",GUILayout.MaxWidth(100f)))
			{
				if(!triggered)
				{
					triggered = true;
					((Trigger)target).envelopes.ForEach(e => e.Trigger());
				}
			}
			else if (triggered)
			{
				triggered = false;
				((Trigger)target).envelopes.ForEach(e => e.Untrigger());
			}
			GUI.color = Color.white;
			base.OnInspectorGUI();
		}
	}
}

