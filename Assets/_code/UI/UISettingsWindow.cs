using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Vopere.Common;

namespace MegaGame.UI
{
    public class UISettingsWindow : MonoBehaviour
    {
        public static UISettingsWindow Instance;

        [SerializeField] GameObject window;

        [Header("Screen Resolution")]
        [SerializeField] int defaultScreenResolutionLevel;
        [SerializeField] GameObject screenResolutionContainer;
        [SerializeField] List<Toggle> screenResolutionToggles = new List<Toggle>();
        [SerializeField] List<TextMeshProUGUI> screenResolutionTexts = new List<TextMeshProUGUI>();

        [Header("Graphics Level")]
        [SerializeField] int defaultGraphicsLevel;
        [SerializeField] List<Toggle> graphicsLevelToggles = new List<Toggle>();

        [Header("Audio")]
        [SerializeField] Slider musicSlider;
        [SerializeField] TextMeshProUGUI musicValueText;
        [SerializeField] Slider UIAudioSlider;
        [SerializeField] TextMeshProUGUI UIAudioValueText;

        [Header("Sensitivity")]
        [SerializeField] Slider movementSensitivitySlider;
        [SerializeField] TextMeshProUGUI movementSensitivityValueText;
        [SerializeField] Slider zoomSensitivitySlider;
        [SerializeField] TextMeshProUGUI zoomSensitivityValueText;

        [Header("Sensitivity")]
        [SerializeField] TextMeshProUGUI appVersionText;

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

        App app;
        DataSaveLoad dataSaveLoad;
        CameraController cameraController;
        AudioController audioController;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UISettingsWindow");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            app = App.Instance;
            dataSaveLoad = DataSaveLoad.Instance;
            cameraController = CameraController.Instance;
            audioController = AudioController.Instance;

            Init();
        }

        public void Init()
        {
            int graphicsLeveId = dataSaveLoad.GetSavedInt("GraphicsLevel");

            if (graphicsLeveId != -1)
                graphicsLevelToggles[graphicsLeveId].isOn = true;
            else
                ChangeGraphicsLevel(defaultGraphicsLevel);

            SetSliderLoadedValue("MusicVolume", musicSlider, musicValueText, musicSlider.maxValue / 2);
            SetSliderLoadedValue("UIVolume", UIAudioSlider, UIAudioValueText, UIAudioSlider.maxValue / 2);

            ChangeMusicVolume();
            ChangeUIVolume();

            SetSliderLoadedValue("MovementSensitivity", movementSensitivitySlider, movementSensitivityValueText, movementSensitivitySlider.maxValue / 2);
            SetSliderLoadedValue("ZoomSensitivity", zoomSensitivitySlider, zoomSensitivityValueText, zoomSensitivitySlider.maxValue / 2);

            SetScreenResolutionSettings();

            if (appVersionText)
                appVersionText.text = Application.version;

            Close();
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);

            if (cameraController)
                cameraController.Freeze(true);

            int graphicsLeveId = dataSaveLoad.GetSavedInt("GraphicsLevel");

            if (graphicsLeveId != -1)
                graphicsLevelToggles[graphicsLeveId].isOn = true;
            else
                graphicsLevelToggles[defaultGraphicsLevel].isOn = true;
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);

            if (cameraController)
                cameraController.Freeze(false);
        }

        void SetSliderLoadedValue(string key, Slider slider, TextMeshProUGUI valueText, float defaultValue)
        {
            float value = dataSaveLoad.GetSavedFloat(key);

            if (value != -1)
                slider.value = value;
            else
                slider.value = defaultValue;

            valueText.text = slider.value.ToString();
        }

        public void ChangeMusicVolume()
        {
            musicValueText.text = musicSlider.value.ToString();

            if (audioController)
                audioController.ChangeVolume(0, musicSlider.value);
        }

        public void ChangeUIVolume()
        {
            UIAudioValueText.text = UIAudioSlider.value.ToString();

            if (audioController)
                audioController.ChangeVolume(1, UIAudioSlider.value);
        }

        public void ChangeSFXVolume()
        {
            if (audioController)
                audioController.ChangeVolume(1, UIAudioSlider.value);
        }

        public void ChangeVoiceVolume()
        {
            if (audioController)
                audioController.ChangeVolume(1, UIAudioSlider.value);
        }

        public void ChangeGraphicsLevel(int id)
        {
            SetGraphicsLevel(id);
            dataSaveLoad.Save("GraphicsLevel", id);
        }

        void SetGraphicsLevel(int id)
        {
            if (app)
                app.SetGraphicsLevel(id);
        }

        public void ChangeResolutionLevel(int id)
        {
            SetResolutionLevel(id);
            dataSaveLoad.Save("ScreenResolution", id);
        }

        void SetResolutionLevel(int id)
        {
            app?.SetResolution(id);
        }

        void SetScreenResolutionSettings()
        {
            int screenResolution = dataSaveLoad.GetSavedInt("ScreenResolution");

            if (screenResolution != -1)
                screenResolutionToggles[screenResolution].isOn = true;
            else
                screenResolutionToggles[defaultScreenResolutionLevel].isOn = true;

            Vector2Int defaultScreenResolution = App.Instance.GetDefaultScreenResolution();

            for (int i = 0; i < screenResolutionTexts.Count; i++)
            {
                if (i == 0)
                    screenResolutionTexts[i].text = defaultScreenResolution.x * 3 / 8 + " x " + defaultScreenResolution.y * 3 / 8;
                else if (i == 1)
                    screenResolutionTexts[i].text = defaultScreenResolution.x / 2 + " x " + defaultScreenResolution.y / 2;
                else if (i == 2)
                    screenResolutionTexts[i].text = defaultScreenResolution.x * 3 / 4 + " x " + defaultScreenResolution.y * 3 / 4;
                else
                    screenResolutionTexts[i].text = defaultScreenResolution.x + " x " + defaultScreenResolution.y;
            }
        }

        public void ChangeMovementSensitivity()
        {
            movementSensitivityValueText.text = movementSensitivitySlider.value.ToString();

            if (cameraController)
                cameraController.ChangeMovementSensitivity(movementSensitivitySlider.value);
        }

        public void ChangeZoomSensitivity()
        {
            zoomSensitivityValueText.text = zoomSensitivitySlider.value.ToString();

            if (cameraController)
                cameraController.ChangeZoomSensitivity(zoomSensitivitySlider.value);
        }

        public void SetLocale(string localeCode)
        {
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

            if (locale != null)
                LocalizationSettings.SelectedLocale = locale;
        }
    }
}
