using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
	Mesh hexMesh;
	MeshCollider meshCollider;

	List<Vector3> vertices;
	List<int> triangles;
	List<Color> colors;

	public void Triangulate(HexCell[] cells)
	{
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();

		foreach (var cell in cells)
		{
			Triangulate(cell);
		}

		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.colors = colors.ToArray();

		hexMesh.RecalculateNormals();

		meshCollider.sharedMesh = hexMesh;
	}

	void Awake()
	{
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";

		meshCollider = gameObject.AddComponent<MeshCollider>();

		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

	void Triangulate(HexCell cell)
	{
		var center = cell.transform.localPosition;

		for (int i = 0; i < 6; i++)
		{
			AddTriangle(center, center + HexMetrics.Corners[i], center + HexMetrics.Corners[i + 1]);
			AddTriangleColor(cell.color);
		}
	}

	void AddTriangle(Vector3 u, Vector3 v, Vector3 w)
	{
		var index = vertices.Count;

		vertices.Add(u, v, w);
		triangles.Add(index, index + 1, index + 2);
	}

	void AddTriangleColor(Color color)
	{
		// Each vertex on a face gets a color
		colors.Add(color, color, color);
	}
}
