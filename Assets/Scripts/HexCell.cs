using UnityEngine;

public class HexCell: MonoBehaviour
{
	//Items
	public HexCoordinates coordinates;

	private int elevation;

	public Color color;

	public RectTransform uiRect; 

	[SerializeField]
	HexCell[] neighbors;

	public int Elevation
	{
		get => elevation;
		set
		{
			elevation = value;
			var position = transform.localPosition;
			position.y = value * HexMetrics.ElevationStep;
			transform.localPosition = position;

			var uiPosition = uiRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.ElevationStep;
			uiRect.localPosition = uiPosition;
		}
	}

	public HexCell GetNeighbor(HexDirection direction)
	{
		return neighbors[(int)direction];
	}

	public void SetNeighbor(HexDirection direction, HexCell cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}
}
