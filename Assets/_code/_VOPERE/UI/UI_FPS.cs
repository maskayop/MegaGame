using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Vopere.UI
{
    public class UI_FPS : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] float sampleDuration = 0.5f;
        [SerializeField] bool isRussian = true;

        int frames = 0;
        float duration;

        string currentLanguage;

        void Start()
        {
            SetFps(0);
        }

        void Update()
        {
            float frameDuration = Time.unscaledDeltaTime;
            frames += 1;
            duration += frameDuration;

            currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;

            if (currentLanguage == "ru")
                isRussian = true;
            else
                isRussian = false;

            if (duration >= sampleDuration)
            {
                SetFps(frames / duration);

                frames = 0;
                duration = 0f;
            }
        }

        void SetFps(float fps)
        {
            if (isRussian)
                text.SetText("КВС: {0:0}", fps);
            else
                text.SetText("FPS: {0:0}", fps);

            if (fps >= 50.0f)
                text.color = Color.green;
            else if (fps >= 29.0f)
                text.color = Color.yellow;
            else
                text.color = Color.red;
        }
    }
}
