using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
	public Color[] colors;

	public HexGrid hexGrid;

	Color activeColor;

	void Awake()
	{
		SelectColor(0);
	}

	void Update()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			HandleInput();
	}

	void HandleInput()
	{
		var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(inputRay, out var hit))
		{
			hexGrid.ColorCell(hit.point, activeColor);
		}
	}

	public void SelectColor(int index)
	{
		activeColor = colors[index];
	}
}
