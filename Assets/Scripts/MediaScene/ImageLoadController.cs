using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoadController : MonoBehaviour
{
	[SerializeField] Image image;

	private Action<OperationResult> OnFinishLoading;
	private string imageLink;

	public void LoadImage(Action<OperationResult> OnFinishLoading, string imageLink)
	{
		this.OnFinishLoading = OnFinishLoading;
		this.imageLink = imageLink;

		StartCoroutine(GetImage());
	}

	private IEnumerator GetImage()
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageLink, nonReadable: false);

		yield return request.SendWebRequest();

		OperationResult operationResult = new OperationResult();
		operationResult.Code = request.responseCode;

		if (request.result == UnityWebRequest.Result.Success)
		{
			Texture2D texture = DownloadHandlerTexture.GetContent(request);
			image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

			SendResultCallback();
			yield break;
		}

		if (request.result == UnityWebRequest.Result.ConnectionError
			|| request.result == UnityWebRequest.Result.DataProcessingError
			|| request.result == UnityWebRequest.Result.ProtocolError
			)
		{
			SendResultCallback($"Network error: {request.error}");
		}
		else
		{
			SendResultCallback($"Unknown error: {request.error}");
		}
	}

	private void SendResultCallback(string errorMessage = null)
	{
		OperationResult result = new OperationResult();
		result.ErrorMessage = errorMessage;

		OnFinishLoading?.Invoke(result);
	}
}
