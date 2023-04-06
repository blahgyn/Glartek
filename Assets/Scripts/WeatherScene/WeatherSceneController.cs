using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class TemperatureUtils
{
	public static float ParseKelvinToCelsius(double value, int digits = 1)
	{
		return (float) Math.Round(value - 273.15f, digits);
	}
}

public class WeatherSceneController : MonoBehaviour
{
	[Header("Scene")]
	[SerializeField] TMP_Dropdown dropdown;
	[SerializeField] Button backBtn;
	[SerializeField] Button testRainBtn;
	[SerializeField] Button testCloudBtn;
	[SerializeField] Button testSunBtn;

	[Header("Cities DB")]
	[Tooltip("Scriptable Object that contains all cities.")]
	[SerializeField] Cities citiesSO;

	[Header("UI")]
	[SerializeField] TextMeshProUGUI tempNow;
	[SerializeField] TextMeshProUGUI tempMin;
	[SerializeField] TextMeshProUGUI tempMax;
	[SerializeField] TextMeshProUGUI date;
	[SerializeField] TextMeshProUGUI errorTxt;
	[SerializeField] TextMeshProUGUI cityNameTxt;

	[Header("Animation")]
	[SerializeField] Transform Sun;
	[SerializeField] Transform Clouds;
	[SerializeField] ParticleSystem Rain;

	private List<City> cities;
	private int selectedCityIndex;

	#region Unity Functions

	void Start()
	{
		SetupScene();
	}

	void OnDestroy()
	{
		backBtn.onClick.RemoveAllListeners();
		testRainBtn.onClick.RemoveAllListeners();
		testCloudBtn.onClick.RemoveAllListeners();
		testSunBtn.onClick.RemoveAllListeners();

		dropdown.onValueChanged.RemoveAllListeners();

		DOTween.KillAll();
	}

	#endregion

	#region Scene Setup

	private void OnCitySelected(int i)
	{
		errorTxt.gameObject.SetActive(false);

		selectedCityIndex = i;
		ClearUITexts();
		DoWeatherInfoRequest();
	}

	private void SetupScene()
	{
		LoadCities();
		SetupDropdown();
		backBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneId.Main));

		testRainBtn.onClick.AddListener(() => {
			ClearUITexts();
			errorTxt.gameObject.SetActive(false);
			StartCoroutine(SetupAnimations(true, false, false));
		});
		testCloudBtn.onClick.AddListener(() => {
			ClearUITexts();
			errorTxt.gameObject.SetActive(false);
			StartCoroutine(SetupAnimations(false, true, false));
		});
		testSunBtn.onClick.AddListener(() => {
			ClearUITexts();
			errorTxt.gameObject.SetActive(false);
			StartCoroutine(SetupAnimations(false, false, true));
		});

		OnCitySelected(0);
	}

	private void LoadCities()
	{
		cities = new List<City>();
		foreach (City c in citiesSO.cities) {
			cities.Add(c);
		}
	}

	private void SetupDropdown()
	{
		List<string> values = new();

		foreach (City c in cities) {
			values.Add(c.Name);
		}

		dropdown.AddOptions(values);
		dropdown.onValueChanged.AddListener(OnCitySelected);
	}

	#endregion

	#region UI Functions

	private void ClearUITexts()
	{
		tempMin.text = "";
		tempMax.text = "";
		tempNow.text = "";

		date.text = "";

		cityNameTxt.text = "";
	}

	private void UpdateTemperatureTexts(float minC, float maxC, float nowC, float minK, float maxK, float nowK)
	{
		tempMin.text = $"{minC}°C ({minK} K)";
		tempMax.text = $"{maxC}°C ({maxK} K)";
		tempNow.text = $"{nowC}°C ({nowK} K)";
	}

	private void UpdateDateTime()
	{
		DateTime currentDate = DateTime.Now;
		string formattedDate = currentDate.ToString("dd-MM-yyyy HH:mm:ss");

		date.text = formattedDate;
	}

	private IEnumerator SetupAnimations(bool isRaining, bool hasClouds, bool isSunny)
	{
		float sunValue    = isSunny || hasClouds ? 1f : -8f; // The sun will show with clouds as well
		float cloudValue  = hasClouds || isRaining ? 1f : -8f; // Clouds will show when raining

		if (isRaining) Rain.Play();
		else Rain.Stop();

		Sun.DOMoveY(sunValue, 1f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(.2f);

		Clouds.DOMoveY(cloudValue, 1f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(.2f);
	}

	#endregion

	#region Weather Info Request

	public void DoWeatherInfoRequest()
	{
		WeatherAPICommunication.Instance.GetWeatherInfo(cities[selectedCityIndex].Id.ToString(), OnSuccess, OnError);
	}

	public void ReadWeatherInfo(WeatherResponse weatherResponse)
	{
		bool isRaining = false;
		bool hasClouds = false;
		bool isSunny = false;

		foreach (Weather weather in weatherResponse.Weather)
		{
			if (weather.Main.Equals("Clouds", StringComparison.OrdinalIgnoreCase)) {
				hasClouds = true;
				continue;
			}
			if (weather.Main.Equals("Rain", StringComparison.OrdinalIgnoreCase)
				|| weather.Main.Equals("Drizzle", StringComparison.OrdinalIgnoreCase)
				|| weather.Main.Equals("Thunderstorm", StringComparison.OrdinalIgnoreCase)
			// || weather.Main.Equals("Snow", StringComparison.OrdinalIgnoreCase)
			) {
				isRaining = true;
				continue;
			}
			if (weather.Main.Equals("Clear", StringComparison.OrdinalIgnoreCase)) {
				isSunny = true;
				continue;
			}
		}

		StartCoroutine(SetupAnimations(isRaining, hasClouds, isSunny));
	}

	public void OnSuccess(WeatherResponse weatherResponse)
	{
		cityNameTxt.text = weatherResponse.Name;

		float tempMinK = (float) weatherResponse.Main.TempMin;
		float tempMaxK = (float) weatherResponse.Main.TempMax;
		float tempK    = (float) weatherResponse.Main.Temp;
		float tempMinC = TemperatureUtils.ParseKelvinToCelsius(tempMinK);
		float tempMaxC = TemperatureUtils.ParseKelvinToCelsius(tempMaxK);
		float tempC    = TemperatureUtils.ParseKelvinToCelsius(tempK);

		UpdateTemperatureTexts(tempMinC, tempMaxC, tempC, tempMinK, tempMaxK, tempK);

		UpdateDateTime();
		ReadWeatherInfo(weatherResponse);
	}

	public void OnError(OperationResult operationResult)
	{
		StartCoroutine(SetupAnimations(false, false, false));
		errorTxt.gameObject.SetActive(true);
		errorTxt.text = $"{operationResult.ErrorMessage} while trying to get informations from {cities[selectedCityIndex].Name}";

#if UNITY_EDITOR
		Debug.LogError(operationResult.ErrorMessage);
#endif
	}

	#endregion
}


