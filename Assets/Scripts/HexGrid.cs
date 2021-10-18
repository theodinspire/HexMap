using System;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
	// Variables
	public int width = 6;
	public int height = 6;

	// Prefabs
	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	// Private items
	HexCell[] cells;
	Canvas gridCanvas;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		
		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
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

		var label = Instantiate(cellLabelPrefab);
		var labelTransform = label.rectTransform;
		labelTransform.SetParent(gridCanvas.transform, false);
		labelTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = $"{x}\n{z}";
	}
}
