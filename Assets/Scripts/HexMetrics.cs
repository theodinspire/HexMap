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
		return (GetFirstCorner(direction) + GetSecondCorner(direction)) * 0.5f * BlendFactor;
	}
}