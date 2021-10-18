using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
	[SerializeField]
	int x, z;

	public int X => x;
	public int Y => -(X + Z);
	public int Z => z;

	public HexCoordinates(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	public static HexCoordinates FromOffsetCoordinates(int x, int z) => new HexCoordinates(x - (z / 2), z);

	public static HexCoordinates FromPosition(Vector3 position)
	{
		var offset = position.z / (HexMetrics.OuterRadius * 3f);
		var foo = position.x / (HexMetrics.InnerRadius * 2f);

		var i = foo - offset;
		var j = -(foo + offset);

		var x = Mathf.RoundToInt(i);
		var y = Mathf.RoundToInt(j);
		var z = Mathf.RoundToInt(-(i + j));

		if (x + y + z != 0)
		{
			var deltaX = Mathf.Abs(i - x);
			var deltaY = Mathf.Abs(j - y);
			var deltaZ = Mathf.Abs(-(i + j + z));

			if (deltaX > deltaY && deltaX > deltaZ)
			{
				return new HexCoordinates(-(y + z), z);
			}

			if (deltaZ > deltaY)
			{
				return new HexCoordinates(x, -(x + y));
			}
		}

		return new HexCoordinates(x, z);
	}

	public override string ToString()
	{
		return $"({X}, {Y}, {Z})";
	}

	public string ToStringOnSeparatesLines()
	{
		return $"{X}\n{Y}\n{Z}";
	}
}