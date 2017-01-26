using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesMove : MonoBehaviour {

	public float speed;
	public float amplitude;
	private Vector2 variableTime;
	Vector3 startPos;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
		variableTime = new Vector2(Random.value, Random.value);
	}
	
	// Update is called once per frame
	void Update ()
	{
		variableTime.x += Random.Range(0.1f, 1f) * Time.deltaTime;
		variableTime.y += Random.Range(0.1f, 1f) * Time.deltaTime;
		transform.position = startPos 
			+ Vector3.right * amplitude * Mathf.Sin(variableTime.x * speed)
			+ Vector3.up    * amplitude * Mathf.Sin(variableTime.y * speed);
	}
}
