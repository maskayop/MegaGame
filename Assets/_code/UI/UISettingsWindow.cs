using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vopere.Common;

namespace MegaGame
{
    public class UISettingsWindow : MonoBehaviour
    {
        public static UISettingsWindow Instance;

        [SerializeField] GameObject window;

        [Header("Screen Resolution")]
        [SerializeField] GameObject screenResolutionContainer;
        [SerializeField] List<Toggle> screenResolutionToggles = new List<Toggle>();
        [SerializeField] List<TextMeshProUGUI> screenResolutionTexts = new List<TextMeshProUGUI>();

        [Header("Graphics Level")]
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

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UISettingsWindow");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Init();
        }

        public void Init()
        {
            int graphicsLeveId = DataSaveLoad.Instance.GetSavedInt("GraphicsLevel");

            if (graphicsLeveId != -1)
                graphicsLevelToggles[graphicsLeveId].isOn = true;

            SetSliderLoadedValue("MusicVolume", musicSlider, musicValueText, 100);
            SetSliderLoadedValue("UIVolume", UIAudioSlider, UIAudioValueText, 100);

            SetSliderLoadedValue("MovementSensitivity", movementSensitivitySlider, movementSensitivityValueText, 5);
            SetSliderLoadedValue("ZoomSensitivity", zoomSensitivitySlider, zoomSensitivityValueText, 7);

            SetScreenResolutionProperties();

            Close();
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);

            if (CameraController.Instance)
                CameraController.Instance.Freeze(true);
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);

            if (CameraController.Instance)
                CameraController.Instance.Freeze(false);
        }

        void SetSliderLoadedValue(string key, Slider slider, TextMeshProUGUI valueText, float defaultValue)
        {
            float value = DataSaveLoad.Instance.GetSavedFloat(key);

            if (value != -1)
                slider.value = value;
            else
                slider.value = defaultValue;

            valueText.text = slider.value.ToString();
        }

        public void ChangeMusicVolume()
        {
            musicValueText.text = musicSlider.value.ToString();

            if (AudioController.Instance)
                AudioController.Instance.ChangeVolume(0, musicSlider.value);
        }

        public void ChangeUIVolume()
        {
            UIAudioValueText.text = UIAudioSlider.value.ToString();

            if (AudioController.Instance)
                AudioController.Instance.ChangeVolume(1, UIAudioSlider.value);
        }

        public void ChangeSFXVolume()
        {
            if (AudioController.Instance)
                AudioController.Instance.ChangeVolume(1, UIAudioSlider.value);
        }

        public void ChangeVoiceVolume()
        {
            if (AudioController.Instance)
                AudioController.Instance.ChangeVolume(1, UIAudioSlider.value);
        }

        public void ChangeGraphicsLevel(int id)
        {
            SetGraphicsLevel(id);
            DataSaveLoad.Instance.Save("GraphicsLevel", id);
        }

        void SetGraphicsLevel(int id)
        {
            if (App.Instance)
                App.Instance.SetGraphicsLevel(id);
        }

        public void ChangeResolutionLevel(int id)
        {
            SetResolutionLevel(id);
            DataSaveLoad.Instance.Save("ScreenResolution", id);
        }

        void SetResolutionLevel(int id)
        {
            if (App.Instance)
                App.Instance.SetResolution(id);
        }

        void SetScreenResolutionProperties()
        {
            int screenResolution = DataSaveLoad.Instance.GetSavedInt("ScreenResolution");

            if (screenResolution != -1)
                screenResolutionToggles[screenResolution].isOn = true;

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

            if (CameraController.Instance)
                CameraController.Instance.ChangeMovementSensitivity(movementSensitivitySlider.value);
        }

        public void ChangeZoomSensitivity()
        {
            zoomSensitivityValueText.text = zoomSensitivitySlider.value.ToString();

            if (CameraController.Instance)
                CameraController.Instance.ChangeZoomSensitivity(zoomSensitivitySlider.value);
        }
    }
}
