using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainSceneController : MonoBehaviour
{
	[Header("Scene")]
	[SerializeField] Button WeatherSceneBtn;
	[SerializeField] Button MediaSceneBtn;
	[SerializeField] Button CubeSceneBtn;
	[SerializeField] Button ExitSceneBtn;
	[SerializeField] Transform MenuTransform;

	#region Unity Functions

	void Start()
	{
		SetupScene();
		AnimateTransform();
	}

	void OnDestroy()
	{
		WeatherSceneBtn.onClick.RemoveAllListeners();
		MediaSceneBtn.onClick.RemoveAllListeners();
		CubeSceneBtn.onClick.RemoveAllListeners();
		ExitSceneBtn.onClick.RemoveAllListeners();

		DOTween.KillAll();
	}

	#endregion

	#region Scene Functions

	private void SetupScene()
	{
		WeatherSceneBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneId.Weather));
		MediaSceneBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneId.Media));
		CubeSceneBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneId.Cube));
		ExitSceneBtn.onClick.AddListener(() => Application.Quit());
	}

	private void AnimateTransform()
	{
		MenuTransform.DOLocalMoveY(0,1f).SetEase(Ease.OutBack);
	}

	#endregion
}
