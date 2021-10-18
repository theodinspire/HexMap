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
		for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
		{
			Triangulate(direction, cell);
		}
	}

	void Triangulate(HexDirection direction, HexCell cell)
	{
		var center = cell.transform.localPosition;
		AddTriangle(
			center,
			center + HexMetrics.GetFirstCorner(direction),
			center + HexMetrics.GetSecondCorner(direction));

		var previous = cell.GetNeighbor(direction.Previous()) ?? cell;
		var neighbor = cell.GetNeighbor(direction) ?? cell;
		var next = cell.GetNeighbor(direction.Next()) ?? cell;

		var edgeColor = (cell.color + neighbor.color) * 0.5f;
		AddTriangleColor(
			cell.color,
			(cell.color + previous.color + neighbor.color) / 3f,
			(cell.color + neighbor.color + next.color) / 3f);
	}

	void AddTriangle(Vector3 u, Vector3 v, Vector3 w)
	{
		var index = vertices.Count;

		vertices.Add(u, v, w);
		triangles.Add(index, index + 1, index + 2);
	}

	void AddTriangleColor(Color a, Color b, Color c)
	{
		// Each vertex on a face gets a color
		colors.Add(a, b, c);
	}
}
