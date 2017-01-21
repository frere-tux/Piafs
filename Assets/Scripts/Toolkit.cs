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

    public static void DrawCurve(float[] data, int channelCount, float scale, float yOffset)
    {
        if (channelCount == 0 || data.Length < 2 * channelCount)
        {
            Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 end = new Vector3(scale, 0.0f, 0.0f);
            Debug.DrawLine(start, end);

            return;
        }

        int sampleCount = data.Length / channelCount;

        for (int channelIndex = 0; channelIndex < channelCount; ++channelIndex)
        {
            Vector3 prevPoint = new Vector3(0.0f, data[channelIndex] + channelIndex * yOffset, 0.0f);
            for (int sampleIndex = 0; sampleIndex < sampleCount; sampleIndex += channelCount)
            {
                Vector3 point = new Vector3(sampleIndex * scale, data[channelIndex + sampleIndex] + channelIndex * yOffset, 0.0f);
                Debug.DrawLine(prevPoint, point, Color.green);
                prevPoint = point;
            }
        }
    }
}
