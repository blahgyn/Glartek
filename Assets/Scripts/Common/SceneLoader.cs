using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public enum SceneId
{
	Main,
	Weather,
	Media,
	Cube,
}

public static class SceneIdExtensions
{
	public static string ToSceneName(this SceneId id)
	{
		switch (id)
		{
			case SceneId.Main:      return "MainScene";
			case SceneId.Weather:   return "WeatherScene";
			case SceneId.Media:     return "MediaScene";
			case SceneId.Cube:      return "CubeScene";
			default: throw new System.Exception($"Unknown {id.GetType()} {id}");
		}
	}
}

/// <summary>
/// Class responsible for async scene transition, fadein and fadeout, and can register callbacks if necessary.
/// </summary>
public class SceneLoader : MonoBehaviour
{
	private static SceneLoader _instance;
	public static SceneLoader Instance => _instance;

	[SerializeField] Image FaderImage;

	void Awake()
	{
		_instance = this;
		GameObject.DontDestroyOnLoad(this);

		FaderImage.raycastTarget = false;
	}

	public static void LoadScene(SceneId sceneName, Action OnBeforeSceneUnload = null, Action OnAfterSceneLoad = null)
	{
		Instance.StartCoroutine(Instance.LoadSceneRoutine(sceneName.ToSceneName(), OnBeforeSceneUnload, OnAfterSceneLoad));
	}

	private IEnumerator LoadSceneRoutine(string sceneName, Action OnBeforeSceneUnload, Action OnAfterSceneLoad)
	{
		OnBeforeSceneUnload?.Invoke();
		yield return FadeOut();

		yield return SceneManager.LoadSceneAsync(sceneName);

		yield return FadeIn();
		OnAfterSceneLoad?.Invoke();
	}

	private IEnumerator FadeIn()
	{
		FaderImage.raycastTarget = false;

		FaderImage.DOFade(0, .3f);
		yield return new WaitForSeconds(.3f);
	}

	private IEnumerator FadeOut()
	{
		FaderImage.raycastTarget = true;

		FaderImage.DOFade(1, .3f);
		yield return new WaitForSeconds(.3f);
	}
}
