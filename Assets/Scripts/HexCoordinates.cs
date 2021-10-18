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

	public override string ToString()
	{
		return $"({X}, {Y}, {Z})";
	}

	public string ToStringOnSeparatesLines()
	{
		return $"{X}\n{Y}\n{Z}";
	}
}