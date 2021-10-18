public enum HexDirection
{
	NE,
	E,
	SE,
	SW,
	W,
	NW,
}

public static class HexDirectionExtensions
{
	public static HexDirection Opposite(this HexDirection direction)
	{
		return (HexDirection)(((int)direction + 3) % 6);
	}

	public static HexDirection Next(this HexDirection direction)
	{
		return (HexDirection)(((int)direction + 1) % 6);
	}

	public static HexDirection Previous(this HexDirection direction)
	{
		return (HexDirection)(((int)direction + 5) % 6);
	}
}