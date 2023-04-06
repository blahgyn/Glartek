using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
	private VideoPlayer videoPlayer;
	private Action<OperationResult> OnFinishLoading;

	private string videoLink;

	#region Unity Functions

	void Awake()
	{
		videoPlayer = GetComponent<VideoPlayer>();
	}

	private void OnDestroy()
	{
		if (videoPlayer != null)
		{
			videoPlayer.prepareCompleted -= OnVideoPrepared;
			videoPlayer.errorReceived    -= OnVideoError;
		}
	}

	#endregion

	#region Video Functions

	/// <summary>
	/// Setup the video player
	/// </summary>
	/// <param name="OnFinishLoading">Callback containing the operation result</param>
	/// <param name="videoLink">URL from video</param>
	public void LoadVideo(Action<OperationResult> OnFinishLoading, string videoLink)
	{
		if (videoPlayer is null) videoPlayer = GetComponent<VideoPlayer>();

		this.videoLink = videoLink;
		this.OnFinishLoading = OnFinishLoading;

		StartCoroutine(SetupVideoPlayerCO());
	}

	private IEnumerator SetupVideoPlayerCO()
	{
		videoPlayer.source = VideoSource.Url;
		videoPlayer.url = videoLink;
		videoPlayer.isLooping = true;
		videoPlayer.sendFrameReadyEvents = false;

		videoPlayer.prepareCompleted += OnVideoPrepared;
		videoPlayer.errorReceived    += OnVideoError;

		videoPlayer.Prepare();
		while (!videoPlayer.isPrepared) yield return new WaitForEndOfFrame();
	}

	public void Play()
	{
		videoPlayer.Play();
	}

	private void OnVideoPrepared(VideoPlayer source)
	{
		SendResultCallback();
	}

	private void OnVideoError(VideoPlayer source, string message)
	{
		SendResultCallback($"Error playing video: {message}");
	}

	private void SendResultCallback(string errorMessage = null)
	{
		OperationResult result = new OperationResult();
		result.ErrorMessage = errorMessage;

		OnFinishLoading?.Invoke(result);
	}

	#endregion
}
