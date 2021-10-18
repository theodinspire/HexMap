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
}