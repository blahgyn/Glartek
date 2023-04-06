using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public enum CubeColors
{
	Red,
	Green,
	Blue
}

public static class CubeColorsExtensions
{
	public static Color ToColor(this CubeColors cubeColor)
	{
		switch (cubeColor)
		{
			case CubeColors.Red:     return Color.red;
			case CubeColors.Green:   return Color.green;
			case CubeColors.Blue:    return Color.blue;
			default: throw new System.Exception($"Unknown {cubeColor.GetType()} {cubeColor}");
		}
	}
}

/// <summary>
/// IComparer to support the .Sort from the list.
/// </summary>
public class CubeColorComparer : IComparer<CubeController>
{
	public int Compare(CubeController x, CubeController y)
	{
		if (x.CubeColor == y.CubeColor) return 0;
		if (x.CubeColor == Color.red)   return -1;
		if (y.CubeColor == Color.red)   return 1;
		if (x.CubeColor == Color.green) return -1;
		return 1;
	}
}

public class CubeController : MonoBehaviour
{
	SpriteRenderer spriteRenderer;

	public Color CubeColor {
		get {
			return spriteRenderer.color;
		}
		private set {
			spriteRenderer.color = value;
		}
	}

	#region Unity Functions

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	#endregion

	#region Cube Functions

	/// <summary>
	/// Setup that sets a random color for the cube
	/// </summary>
	public void Setup()
	{
		if (spriteRenderer is null) spriteRenderer = GetComponent<SpriteRenderer>();

		SetRandomColor();
	}

	/// <summary>
	/// Setup that clones another cube colors
	/// </summary>
	public void Setup(Color color)
	{
		if (spriteRenderer is null) spriteRenderer = GetComponent<SpriteRenderer>();

		CubeColor = color;
	}

	private void SetRandomColor()
	{
		Array values = Enum.GetValues(typeof(CubeColors));
        System.Random random = new();
		CubeColors randomBar = (CubeColors)values.GetValue(random.Next(values.Length));
		CubeColor = randomBar.ToColor();
	}

	/// <summary>
	/// Move the Cube for the respective CubeList and position (the index in the list)
	/// </summary>
	public void MoveToCubeList(CubeList cubeList, int position)
	{
		float collumAxu = position % CubeSceneController.MaxLineSize;
		float lineAxu   = position / CubeSceneController.MaxLineSize;
		transform.DOMove(new Vector3(-5 + collumAxu * .4f, (-lineAxu * .4f) + cubeList.ToWorldPositionY(), 0), .1f);
	}

	#endregion
}
