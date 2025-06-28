using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System; // �����̴��� �ؽ�Ʈ�� �ʿ�

public class GameSettingsManager : MonoBehaviour
{
    public GameObject gameSettingPanel;

    [Header("Resolution Settings")]
    public Resolution[] Resolutions;
    public GameObject ResolutionDrop;
    public Toggle FullScreen;
    TMP_Dropdown ResolutionDropdown;
    float val = 100f;
    int resolutionNum;

    [Header("Sound Settings")]
    public Slider volumeSlider;     // ���� �����̴�
    void Start()
    {
        ResolutionDropdown = ResolutionDrop.GetComponent<TMP_Dropdown>();
        ResetSettings();
        Resolutions = Screen.resolutions;
        SetVolume();
        foreach (Resolution resolution in Resolutions)
        {
            var MAX = GCD(resolution.width, resolution.height);
            string[] resarr = resolution.ToString().Split(' ');
            ResolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{resarr[0]} * {resarr[2]}"));
        }
        for (int i = 0; i < ResolutionDropdown.options.Count; i++)
        {
            string[] Arr = ResolutionDropdown.options[i].text.Split(' ');
            if (int.Parse(Arr[0]) == Screen.width)
            {
                ResolutionDropdown.value = i;
                resolutionNum = i;
                break;
            }
        }

        SetSettings();

    }
    public Text volumeText;         // % ǥ�� �ؽ�Ʈ
    public AudioMixer masterMixer;  // ������ ����� �ͼ� (Unity ������Ʈ�� �����Ǿ� �־�� ��)

    int GCD(int a, int b)
    {
        if (b == 0) return a;
        else return GCD(b, a % b);
    }

    public void CloseGameSettingPanel()
    {
        gameSettingPanel.SetActive(false);
        CloseSettings();
    }

    public void SetSettings()
    {
        string Resolution = ResolutionDropdown.options[ResolutionDropdown.value].text;
        string[] ResArr = Resolution.Split(' ');
        resolutionNum = ResolutionDropdown.value;
        Screen.SetResolution(int.Parse(ResArr[0].Trim()), int.Parse(ResArr[2].Trim()), FullScreen.isOn);
        PlayerPrefs.SetInt("ScreenWidth", int.Parse(ResArr[0].Trim()));
        PlayerPrefs.SetInt("ScreenHeight", int.Parse(ResArr[2].Trim()));
        PlayerPrefs.SetString("FullActive", Convert.ToString(FullScreen.isOn));
        PlayerPrefs.SetFloat("VolumeValue", val);
    }

    public void SetVolume()
    {
        // ���� ���� -80 ~ 0 dB �� ��ȯ�ؼ� �ͼ��� ����
        val = volumeSlider.value;
        float dB = Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
        UpdateVolumeUI(val);
    }

    private void UpdateVolumeUI(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeText.text = $"{percent}%";
    }

    void ResetSettings()
    {
        if (PlayerPrefs.HasKey("ScreenWidth"))
        {
            FullScreen.isOn = Convert.ToBoolean(PlayerPrefs.GetString("FullActive"));
            Screen.SetResolution(PlayerPrefs.GetInt("ScreenWidth"), PlayerPrefs.GetInt("ScreenHeight"), FullScreen.isOn);
        }
        if (PlayerPrefs.HasKey("VolumeValue"))
        { 
            val = PlayerPrefs.GetFloat("VolumeValue");
            volumeSlider.value = val;
        }
    }

    void CloseSettings()
    {
        ResetSettings();
        SetVolume();
        ResolutionDropdown.value = resolutionNum;
        
    }
}
