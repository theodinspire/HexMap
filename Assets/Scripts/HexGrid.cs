using System;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
	// Variables
	public int width = 6;
	public int height = 6;

	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

	// Prefabs
	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	// Private items
	HexCell[] cells;
	Canvas gridCanvas;
	HexMesh hexMesh;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	void Start()
	{
		hexMesh.Triangulate(cells);
	}

	void CreateCell(int x, int z, int i)
	{
		var position = new Vector3
		{
			x = (x + z * 0.5f - (int)(z / 2)) * (HexMetrics.InnerRadius * 2f),
			y = 0,
			z = z * (HexMetrics.OuterRadius * 1.5f),
		};

		var cell = cells[i] = Instantiate(cellPrefab);

		var cellTransform = cell.transform;
		cellTransform.SetParent(transform, false);
		cellTransform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;

		if (x > 0)
		{
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}

		if (z > 0)
		{
			if ((z & 1) == 0)
			{
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else
			{
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1)
				{
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
			}
		}

		var label = Instantiate(cellLabelPrefab);
		var labelTransform = label.rectTransform;
		labelTransform.SetParent(gridCanvas.transform, false);
		labelTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparatesLines();
	}

	public void ColorCell(Vector3 position, Color color)
	{
		var location = transform.InverseTransformPoint(position);
		var coordinates = HexCoordinates.FromPosition(location);
		var index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

		var cell = cells[index];
		cell.color = color;
		hexMesh.Triangulate(cells);
	}
}
