using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelControls : MonoBehaviour
{
    public abstract bool IsSolved();

	public override string ToString()
	{
		return name;
	}
}
