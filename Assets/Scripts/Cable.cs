using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Cable : MonoBehaviour
{
	public GameObject start, end;
	public Vector3 startDir, endDir;
	public int cableResolution;
	public float cableWidth = 0.3f;
	public bool synthCableBehavior;
	public int sortingLayerOrder;
	public string sortingLayerName;

	public Vector3 startPos, endPos;
	private float dist;
	private MeshFilter meshFilter;
	private Mesh builtMesh;
	private Vector3[] cablePoints;

	// Use this for initialization
	void Start ()
	{
		meshFilter = GetComponent<MeshFilter>();
		BuildMesh();
		RefreshMesh();
		GetComponent<MeshRenderer>().sortingOrder = sortingLayerOrder;
		GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
		//start.transform.SetParent(null, true);
		//end.transform.SetParent(null, true);
		//transform.position = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(start.transform.position != startPos || end.transform.position != endPos)
		{
			startPos = start.transform.position;
			endPos = end.transform.position;
			endPos.z = startPos.z;
			dist = Vector3.Distance(startPos, endPos);
			if(synthCableBehavior)
			{
				startDir = dist * Vector3.up * 0.5f;
				endDir = dist * Vector3.up * -0.5f;
			}
			RefreshMesh();
		}
	}

	void BuildMesh()
	{
		builtMesh = new Mesh();
		builtMesh.name = "CableMesh";
		int pointCount = cableResolution;
		Vector3[] vert = new Vector3[pointCount * 2];
		for (int i = 0; i < pointCount; i++)
		{
			vert[i * 2 + 0] = Vector3.right * i;
			vert[i * 2 + 1] = Vector3.right * i + Vector3.up;
		}
		int[] tri = new int[(pointCount - 1) * 2 * 3];
		for (int i = 0; i < pointCount - 1; i++)
		{
			tri[i * 6 + 0] = i*2 + 0;
			tri[i * 6 + 1] = i*2 + 3;
			tri[i * 6 + 2] = i*2 + 1;
			tri[i * 6 + 3] = i*2 + 0;
			tri[i * 6 + 4] = i*2 + 2;
			tri[i * 6 + 5] = i*2 + 3;
		}
		Vector2[] uv = new Vector2[pointCount * 2];
		for (int i = 0; i < pointCount - 1; i++)
		{
			uv[i * 2 + 0] = new Vector2(0f, (float)i / (float)(pointCount));
			uv[i * 2 + 1] = new Vector2(1f, (float)i / (float)(pointCount));
		}
		builtMesh.vertices = vert;
		builtMesh.triangles = tri;
		builtMesh.uv = uv;
		builtMesh.MarkDynamic();
		meshFilter.mesh = builtMesh;
	}

	void RefreshMesh()
	{
		int pointCount = cableResolution;
		cablePoints = new Vector3[pointCount];
		Vector3[] vert = new Vector3[pointCount * 2];

		for (int i = 0; i < pointCount; i++)
		{
			cablePoints[i] = Toolkit.CalculateBezierPoint(i / (pointCount - 1f),
								startPos,
								startPos + startDir,
								endPos + endDir,
								endPos);
			Vector3 orthogonal, cableDir;
			if(i <= 0)
			{
				cableDir = startDir.normalized;
			}
			else
			{
				cableDir = (cablePoints[i] - cablePoints[i - 1]).normalized;
			}
			orthogonal = Vector3.Cross(cableDir, Vector3.forward).normalized;

			vert[i * 2 + 0] = cablePoints[i] - orthogonal * cableWidth - transform.position;
			vert[i * 2 + 1] = cablePoints[i] + orthogonal * cableWidth - transform.position;
		}
		Vector2[] uv = new Vector2[pointCount * 2];
		for (int i = 0; i < pointCount - 1; i++)
		{
			float p = (pointCount - i) / (float)pointCount;
			float d = Mathf.Pow(dist * 0.1f, 0.8f);
			uv[i * 2 + 0] = new Vector2(0f, d * p);
			uv[i * 2 + 1] = new Vector2(1f, d * p);
		}
		builtMesh.uv = uv;
		builtMesh.vertices = vert;
		builtMesh.RecalculateBounds();
	}

	void OnDrawGizmos()
	{
		if (start) Gizmos.DrawCube(start.transform.position, Vector3.one * 0.1f);
		if (end) Gizmos.DrawCube(end.transform.position, Vector3.one * 0.1f);
	}
}
