using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Toolkit {

	public static float VoltToDb(float volume, float referenceVolume)
	{
		return 20f * Mathf.Log10(volume / referenceVolume);
	}

	public static float DbToVolt(float decibels)
	{
		return Mathf.Pow(10f, decibels / 20f);
	}
}
