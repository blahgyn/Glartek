using UnityEngine;

/// <summary>
/// Class that creates the persistend [Application] object.
/// </summary>
public class ApplicationInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnBeforeSceneLoadRuntimeMethod()
	{
		CreateApplicationObject();
	}

	private static void CreateApplicationObject()
	{
		var applicationPrefab = Resources.Load<GameObject>("Prefabs/Application");
		var applicationGo = GameObject.Instantiate(applicationPrefab);
		applicationGo.name = "[Application]";
		GameObject.DontDestroyOnLoad(applicationGo);
	}
}