using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MediaSceneController : MonoBehaviour
{
	[Header("Scene")]
	[SerializeField] Button startBtn;
	[SerializeField] Button backBtn;
	[SerializeField] Button imgErrorBtn;
	[SerializeField] Button vidErrorBtn;

	[Header("Media Objects")]
	[SerializeField] Image videoFader;
	[SerializeField] Image videoLoadingSymbol;
	[SerializeField] Image videoErrorSymbol;
	[SerializeField] TextMeshProUGUI videoErrorTxt;
	[SerializeField] Image imageFader;
	[SerializeField] Image imageLoadingSymbol;
	[SerializeField] Image imageErrorSymbol;
	[SerializeField] TextMeshProUGUI imageErrorTxt;

	[Header("Controllers")]
	[SerializeField] VideoPlayerController videoPlayerController;
	[SerializeField] ImageLoadController imageLoadController;

	private string videoURL = "http://www.rogerioblesa.com/test/video.mp4";
	private string imageURL = "http://www.rogerioblesa.com/test/image.JPEG";

	#region Unity Functions

	void Start()
	{
		SetupScene();
	}

	private void OnDestroy()
	{
		startBtn.onClick.RemoveAllListeners();
		backBtn.onClick.RemoveAllListeners();
		imgErrorBtn.onClick.RemoveAllListeners();
		vidErrorBtn.onClick.RemoveAllListeners();

		DOTween.KillAll();
	}

	#endregion

	#region Scene Functions

	private void SetupScene()
	{
		startBtn.onClick.AddListener(OnStartBtnClicked);
		imgErrorBtn.onClick.AddListener(() => {
			imageURL = "invalidURL";
			OnStartBtnClicked();
		});
		vidErrorBtn.onClick.AddListener(() => {
			videoURL = "invalidURL";
			OnStartBtnClicked();
		});

		backBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneId.Main));
	}

	public void OnStartBtnClicked()
	{
		videoLoadingSymbol.transform.DORotate(new Vector3(0,0,-360), 1f).SetLoops(-1).SetEase(Ease.Linear).SetRelative();
		imageLoadingSymbol.transform.DORotate(new Vector3(0,0,-360), 1f).SetLoops(-1).SetEase(Ease.Linear).SetRelative();
		videoLoadingSymbol.gameObject.SetActive(true);
		imageLoadingSymbol.gameObject.SetActive(true);

		startBtn.interactable = false;
		imgErrorBtn.interactable = false;
		vidErrorBtn.interactable = false;

		GetImage(imageURL);
		GetVideo(videoURL);
	}

	#endregion

	#region Media Functions

	private void GetVideo(string mediaLink)
	{
		videoPlayerController.LoadVideo(OnFinishVideoLoading, mediaLink);
	}

	private void GetImage(string mediaLink)
	{
		imageLoadController.LoadImage(OnFinishImageLoading, mediaLink);
	}

	public void OnFinishVideoLoading(OperationResult operationResult)
	{
		videoLoadingSymbol.gameObject.SetActive(false);

		if (operationResult.Success) {
			videoPlayerController.Play();
			videoFader.DOFade(0,1f);
		}
		else {
			videoErrorSymbol.gameObject.SetActive(true);
			videoErrorTxt.text = operationResult.ErrorMessage;
		}
	}

	public void OnFinishImageLoading(OperationResult operationResult)
	{
		imageLoadingSymbol.gameObject.SetActive(false);

		if (operationResult.Success) {
			imageFader.DOFade(0,1f);
		}
		else {
			imageErrorSymbol.gameObject.SetActive(true);
			imageErrorTxt.text = operationResult.ErrorMessage;
		}
	}

	#endregion
}
