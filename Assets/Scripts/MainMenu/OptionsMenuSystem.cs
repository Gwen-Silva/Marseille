using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuSystem : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] private GameObject optionsPanel;

	[Header("UI")]
	[SerializeField] private Slider volumeSlider;
	[SerializeField] private TMP_Dropdown resolutionDropdown;

	private Resolution[] availableResolutions;

	private void Start()
	{
		SetupVolume();
		SetupResolutions();
	}
	public void OpenOptions()
	{
		optionsPanel.SetActive(true);
	}

	public void CloseOptions()
	{
		optionsPanel.SetActive(false);
	}

	private void SetupVolume()
	{
		volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
		volumeSlider.onValueChanged.AddListener(SetVolume);
		SetVolume(volumeSlider.value);
	}

	private void SetVolume(float value)
	{
		AudioListener.volume = value;
		PlayerPrefs.SetFloat("Volume", value);
	}

	private void SetupResolutions()
	{
		availableResolutions = Screen.resolutions;
		resolutionDropdown.ClearOptions();

		int currentResolutionIndex = 0;
		var options = new System.Collections.Generic.List<string>();

		for (int i = 0; i < availableResolutions.Length; i++)
		{
			var res = availableResolutions[i];
			string label = res.width + " x " + res.height;
			options.Add(label);

			if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
				currentResolutionIndex = i;
		}

		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();
		resolutionDropdown.onValueChanged.AddListener(SetResolution);
	}

	private void SetResolution(int index)
	{
		Resolution res = availableResolutions[index];
		Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed);
	}
}
