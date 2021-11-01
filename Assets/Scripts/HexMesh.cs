using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
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

		if (direction <= HexDirection.SE)
			TriangulateConnection(direction, cell, t, u);
	}

	void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 t, Vector3 u)
	{
		var neighbor = cell.GetNeighbor(direction);
		if (neighbor == null) return;

		var bridge = HexMetrics.GetBridge(direction);

		var v = t + bridge;
		var w = u + bridge;
		v.y = w.y = neighbor.Elevation * HexMetrics.ElevationStep;

		if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
		{
			TriangulateEdgeTerraces(t, u, cell, v, w, neighbor);
		}
		else
		{
			AddQuad(t, u, v, w);
			AddQuadColor(cell.color, neighbor.color);
		}

		if (direction > HexDirection.E) return;

		var nextDirection = direction.Next();
		var next = cell.GetNeighbor(nextDirection);

		if (next == null) return;

		var x = u + HexMetrics.GetBridge(nextDirection);
		x.y = next.Elevation * HexMetrics.ElevationStep;

		// TODO: These logic branches are a fucking mess
		if (cell.Elevation <= neighbor.Elevation)
		{
			if (cell.Elevation <= next.Elevation)
			{
				TriangulateCorner(u, cell, w, neighbor, x, next);
			}
			else
			{
				TriangulateCorner(x, next, u, cell, w, neighbor);
			}
		}
		else if (neighbor.Elevation <= next.Elevation)
		{
			TriangulateCorner(w, neighbor, x, next, u, cell);
		}
		else
		{
			TriangulateCorner(x, next, u, cell, w, neighbor);
		}
	}

	void TriangulateEdgeTerraces(
		Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
		Vector3 endLeft, Vector3 endRight, HexCell endCell)
	{
		var v = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
		var w = HexMetrics.TerraceLerp(beginRight, endRight, 1);
		var nextColor = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

		AddQuad(beginLeft, beginRight, v, w);
		AddQuadColor(beginCell.color, nextColor);

		for (int i = 2; i < HexMetrics.TerraceSteps; i++)
		{
			var t = v;
			var u = w;
			var previousColor = nextColor;

			v = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
			w = HexMetrics.TerraceLerp(beginRight, endRight, i);
			nextColor = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);

			AddQuad(t, u, v, w);
			AddQuadColor(previousColor, nextColor);
		}

		AddQuad(v, w, endLeft, endRight);
		AddQuadColor(nextColor, endCell.color);
	}

	void TriangulateCorner(
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell)
	{
		var leftEdgeType = bottomCell.GetEdgeType(leftCell);
		var rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.Slope)
		{
			if (rightEdgeType == HexEdgeType.Slope)
			{
				TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
			}
			else if (rightEdgeType == HexEdgeType.Flat)
			{
				TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
			}
			else
			{
				TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
			}
		}
		else if (rightEdgeType == HexEdgeType.Slope)
		{
			if (leftEdgeType == HexEdgeType.Flat)
			{
				TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
			}
			else
			{
				TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			if (leftCell.Elevation < rightCell.Elevation)
			{
				TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
			}
			else
			{
				TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
			}
		}
		else
		{
			AddTriangle(bottom, left, right);
			AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
		}
	}

	void TriangulateCornerTerraces(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell)
	{
		var v = HexMetrics.TerraceLerp(begin, left, 1);
		var w = HexMetrics.TerraceLerp(begin, right, 1);
		var nextLeftColor = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
		var nextRightColor = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

		AddTriangle(begin, v, w);
		AddTriangleColor(beginCell.color, nextLeftColor, nextRightColor);

		for (int i = 2; i < HexMetrics.TerraceSteps; ++i)
		{
			var t = v;
			var u = w;

			var previousLeftColor = nextLeftColor;
			var previousRightColor = nextRightColor;

			v = HexMetrics.TerraceLerp(begin, left, i);
			w = HexMetrics.TerraceLerp(begin, right, i);
			nextLeftColor = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			nextRightColor = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);

			AddQuad(t, u, v, w);
			AddQuadColor(previousLeftColor, previousRightColor, nextLeftColor, nextRightColor);
		}

		AddQuad(v, w, left, right);
		AddQuadColor(nextLeftColor, nextRightColor, leftCell.color, rightCell.color);
	}

	void TriangulateCornerTerracesCliff(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell)
	{
		var b = 1f / (rightCell.Elevation - beginCell.Elevation);
		if (b < 0) b = -b;
		var boundary = Vector3.Lerp(begin, right, b);
		var boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
		}
		else
		{
			AddTriangle(left, right, boundary);
			AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}

	void TriangulateCornerCliffTerraces(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell)
	{
		var b = 1f / (leftCell.Elevation - beginCell.Elevation);
		if (b < 0) b = -b;
		var boundary = Vector3.Lerp(begin, left, b);
		var boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

		TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
		}
		else
		{
			AddTriangle(left, right, boundary);
			AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}

	void TriangulateBoundaryTriangle(
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor)
	{
		var v = HexMetrics.TerraceLerp(begin, left, 1);
		var nextColor = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		AddTriangle(begin, v, boundary);
		AddTriangleColor(beginCell.color, nextColor, boundaryColor);

		for (var i = 2; i < HexMetrics.TerraceSteps; ++i)
		{
			var u = v;
			var previousColor = nextColor;

			v = HexMetrics.TerraceLerp(begin, left, i);
			nextColor = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);

			AddTriangle(u, v, boundary);
			AddTriangleColor(previousColor, nextColor, boundaryColor);
		}

		AddTriangle(v, left, boundary);
		AddTriangleColor(nextColor, leftCell.color, boundaryColor);
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
