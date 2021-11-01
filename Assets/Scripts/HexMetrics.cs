using UnityEngine;

public static class HexMetrics
{
	public const float OuterRadius = 10f;

	public const float InnerRadius = OuterRadius * 0.866025404f;

	public const float SolidFactor = 0.8f;
	public const float BlendFactor = 1f - SolidFactor;

	static Vector3[] Corners =
	{
		new Vector3(          0f, 0f,         OuterRadius),
		new Vector3( InnerRadius, 0f,  0.5f * OuterRadius),
		new Vector3( InnerRadius, 0f, -0.5f * OuterRadius),
		new Vector3(          0f, 0f,        -OuterRadius),
		new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
		new Vector3(-InnerRadius, 0f,  0.5f * OuterRadius),
		new Vector3(          0f, 0f,         OuterRadius),
	};

	public const float ElevationStep = 2f;

	public const int TerracesPerSlope = 2;

	public const int TerraceSteps = TerracesPerSlope * 2 + 1;

	public const float HorizontalTerraceStepSize = 1f / TerraceSteps;
	public const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

	public static Vector3 GetFirstCorner(HexDirection direction)
	{
		return Corners[(int)direction];
	}

	public static Vector3 GetSecondCorner(HexDirection direction)
	{
		return Corners[(int)direction + 1];
	}

	public static Vector3 GetFirstSolidCorner(HexDirection direction)
	{
		return GetFirstCorner(direction) * SolidFactor;
	}

	public static Vector3 GetSecondSolidCorner(HexDirection direction)
	{
		return GetSecondCorner(direction) * SolidFactor;
	}

	public static Vector3 GetBridge(HexDirection direction)
	{
		return (GetFirstCorner(direction) + GetSecondCorner(direction)) * BlendFactor;
	}

	public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
	{
		var h = step * HorizontalTerraceStepSize;
		var v = (int)((step + 1) / 2) * VerticalTerraceStepSize;

		return new Vector3
		{
			x = a.x + (b.x - a.x) * h,
			y = a.y + (b.y - a.y) * v,
			z = a.z + (b.z - a.z) * h
		};
	}

	public static Color TerraceLerp(Color a, Color b, int step)
	{
		var h = step * HorizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}
}