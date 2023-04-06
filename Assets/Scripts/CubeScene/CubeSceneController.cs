using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CubeList
{
	Unsorted,
	Sorted,
	Red,
	Green,
	Blue
}

public static class CubeListExtensions
{
	/// <summary>
	/// Extension responsible to find the World Y position (arbitrary) for each CubeList
	/// </summary>
	public static float ToWorldPositionY(this CubeList cubeList)
	{
		switch (cubeList)
		{
			case CubeList.Unsorted:  return 3;
			case CubeList.Sorted:    return 1.5f;
			case CubeList.Red:       return 0;
			case CubeList.Green:     return -1f;
			case CubeList.Blue:      return -2;
			default: throw new System.Exception($"Unknown {cubeList.GetType()} {cubeList}");
		}
	}
}

public class CubeSceneController : MonoBehaviour
{
	[Header("Prefab")]
	[SerializeField] CubeController cubePrefab;

	[Header("Scene Objects")]
	[SerializeField] Transform cubeSpawnLocation;

	[Header("UI")]
	[SerializeField] Button OrderBtn;
	[SerializeField] Button FastForwardBtn;
	[SerializeField] Button BackBtn;

	// Max number of cubes in one line (arbitrary value)
	public static int MaxLineSize = 32;

	private List<CubeController> UnsortedCubes;
	private List<CubeController> SortedCubes;
	private List<CubeController> RedCubes;
	private List<CubeController> GreenCubes;
	private List<CubeController> BlueCubes;

	// Time to wait when animating each cube
	private float CurrentAnimationSpeed = .5f;

	private const int NOfCubes = 64;

	#region Unity Functions

	void Start()
	{
		SetupScene();
		StartCoroutine(CreateCubes());
	}

	void OnDestroy()
	{
		OrderBtn.onClick.RemoveAllListeners();
		BackBtn.onClick.RemoveAllListeners();
		FastForwardBtn.onClick.RemoveAllListeners();

		DOTween.KillAll();
	}

	#endregion

	#region Scene Functions

	private void SetupScene()
	{
		OrderBtn.interactable = false;
		OrderBtn.onClick.AddListener(OnStartSortBtnClicked);
		FastForwardBtn.interactable = false;
		FastForwardBtn.onClick.AddListener(OnFastForwardBtnClicked);

		UnsortedCubes = new List<CubeController>();
		SortedCubes   = new List<CubeController>();
		RedCubes      = new List<CubeController>();
		GreenCubes    = new List<CubeController>();
		BlueCubes     = new List<CubeController>();

		BackBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneId.Main));
	}

	public void OnStartSortBtnClicked()
	{
		FastForwardBtn.interactable = true;
		OrderBtn.interactable = false;

		StartCoroutine(OrderByColor());
	}

	public void OnFastForwardBtnClicked()
	{
		FastForwardBtn.interactable = false;
		CurrentAnimationSpeed = .05f;
	}

	#endregion

	#region Cube Functions

	private IEnumerator CreateCubes()
	{
		for (int i = 0; i < NOfCubes; i++)
		{
			// Create cube in the Unsorted List
			CubeController cube = Instantiate(cubePrefab, cubeSpawnLocation);
			UnsortedCubes.Add(cube);
			cube.Setup();

			// Create cube in the Sorted List (still unsorted)
			CubeController cubeClone = Instantiate(cubePrefab, cubeSpawnLocation);
			SortedCubes.Add(cubeClone);
			cubeClone.Setup(cube.CubeColor);

			// Animate the cube movement to each list
			cube.MoveToCubeList(CubeList.Unsorted, UnsortedCubes.IndexOf(cube));
			cubeClone.MoveToCubeList(CubeList.Sorted, SortedCubes.IndexOf(cubeClone));

			yield return new WaitForSeconds(.03f);
		}

		OrderBtn.interactable = true;
	}

	private IEnumerator OrderByColor()
	{
		// First it move the cubes from the Unsorted List to the respective colors list
		foreach (CubeController cube in UnsortedCubes)
		{
			yield return new WaitForSeconds(CurrentAnimationSpeed);

			if (cube.CubeColor == Color.red)
			{
				RedCubes.Add(cube);
				cube.MoveToCubeList(CubeList.Red, RedCubes.IndexOf(cube));
				continue;
			}
			if (cube.CubeColor == Color.green)
			{
				GreenCubes.Add(cube);
				cube.MoveToCubeList(CubeList.Green, GreenCubes.IndexOf(cube));
				continue;
			}
			if (cube.CubeColor == Color.blue)
			{
				BlueCubes.Add(cube);
				cube.MoveToCubeList(CubeList.Blue, BlueCubes.IndexOf(cube));
				continue;
			}
		}

		// After move to the respective lists, clear the Usorted List
		UnsortedCubes.Clear();

		yield return new WaitForSeconds(1f);

		// Order the list
		SortedCubes.Sort(new CubeColorComparer());

		// Move each cube to its ordered position
		foreach (CubeController c in SortedCubes)
		{
			c.MoveToCubeList(CubeList.Sorted, SortedCubes.IndexOf(c));
			yield return new WaitForSeconds(.03f);
		}
	}

	#endregion








}
