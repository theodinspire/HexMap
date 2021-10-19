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
		var t = center + HexMetrics.GetFirstSolidCorner(direction);
		var u = center + HexMetrics.GetSecondSolidCorner(direction);

		AddTriangle(center, t, u);
		AddTriangleColor(cell.color);

		var bridge = HexMetrics.GetBridge(direction);
		var v = t + bridge;
		var w = u + bridge;

		AddQuad(t, u, v, w);

		var previous = cell.GetNeighbor(direction.Previous()) ?? cell;
		var neighbor = cell.GetNeighbor(direction) ?? cell;
		var next = cell.GetNeighbor(direction.Next()) ?? cell;


		var bridgeColor = (cell.color + neighbor.color) * 0.5f;
		AddQuadColor(cell.color, bridgeColor);

		AddTriangle(t, center + HexMetrics.GetFirstCorner(direction), v);
		AddTriangleColor(cell.color, (cell.color + previous.color + neighbor.color) / 3f, bridgeColor);

		AddTriangle(u, w, center + HexMetrics.GetSecondCorner(direction));
		AddTriangleColor(cell.color, bridgeColor, (cell.color + neighbor.color + next.color) / 3f);
	}

	void AddTriangle(Vector3 u, Vector3 v, Vector3 w)
	{
		var index = vertices.Count;

		vertices.Add(u, v, w);
		triangles.Add(index, index + 1, index + 2);
	}

	void AddQuad(Vector3 t, Vector3 u, Vector3 v, Vector3 w)
	{
		var index = vertices.Count;

		vertices.Add(t, u, v, w);
		triangles.Add(index, index + 2, index + 1, index + 1, index + 2, index + 3);
	}

	void AddTriangleColor(Color a, Color b, Color c)
	{
		// Each vertex on a face gets a color
		colors.Add(a, b, c);
	}

	void AddTriangleColor(Color color)
	{
		// Each vertex on a face gets a color
		colors.Add(color, color, color);
	}

	void AddQuadColor(Color a, Color b, Color c, Color d)
	{
		colors.Add(a, b, c, d);
	}

	void AddQuadColor(Color a, Color b)
	{
		colors.Add(a, a, b, b);
	}
}
