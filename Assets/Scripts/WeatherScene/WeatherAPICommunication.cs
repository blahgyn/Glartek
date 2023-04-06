using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherAPICommunication : MonoBehaviour
{
	private const string APIKey      = "a33bcf527929681051af5a067bcc0573";
	private const string APIAddress  = "api.openweathermap.org/data/";
	private string APIVersion        = "2.5";

	// Maximum time in seconds to wait for a request
	private const int requestTimeout = 3;

	private static WeatherAPICommunication _instance;
	public static WeatherAPICommunication Instance => _instance;

	public Action<WeatherResponse> OnSuccess;
	public Action<OperationResult> OnError;

	#region Unity Functions

	void Awake()
	{
		_instance = this;
	}

	#endregion

	#region API Functions

	public void GetWeatherInfo(string cityId, Action<WeatherResponse> OnSuccess, Action<OperationResult> OnError = null)
	{
		this.OnSuccess = OnSuccess;
		this.OnError = OnError;

		StartCoroutine(DoRequest(GetAPILink(cityId)));
	}

	private string GetAPILink(string cityID)
	{
		return APIAddress + APIVersion + "/" + "weather?id=" + cityID + "&APPID=" + APIKey;
	}

	private void DeserializeWeatherInfo(OperationResult operationResult)
	{
		if (!operationResult.Success) {
			return;
		}

		string responseText = operationResult.ResultData;

		WeatherResponse response;
		try {
			response = DeserializeJson<WeatherResponse>(responseText);
		} catch (Exception e){
			operationResult.ErrorMessage = $"Deserialize error: {e.Message}";
			OnError?.Invoke(operationResult);
			return;
		}

		OnSuccess?.Invoke(response);
	}

	private IEnumerator DoRequest(string link)
	{
		UnityWebRequest request = UnityWebRequest.Get(link);
		request.timeout = requestTimeout;

		yield return request.SendWebRequest();

		OperationResult operationResult = new OperationResult();
		operationResult.Code = request.responseCode;

		if (request.result == UnityWebRequest.Result.Success)
		{
			operationResult.ResultData = request.downloadHandler.text;
			DeserializeWeatherInfo(operationResult);
			yield break;
		}

		if (request.result == UnityWebRequest.Result.ConnectionError
			|| request.result == UnityWebRequest.Result.DataProcessingError
			|| request.result == UnityWebRequest.Result.ProtocolError
			)
		{
			operationResult.ErrorMessage = $"Network error: {request.error}";
		}
		else
		{
			operationResult.ErrorMessage = $"Unknown error: {request.error}";
		}

		OnError?.Invoke(operationResult);
	}

	private T DeserializeJson<T>(string json) where T : class
	{
		return JsonConvert.DeserializeObject<T>(json);
	}

	#endregion
}