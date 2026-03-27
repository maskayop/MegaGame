using RuStore.PayClient;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIPurchaseMethodBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Dropdown theme;

        Action<SdkTheme> onPreferredOneStep;
        Action<SdkTheme> onPreferredTwoStep;
        Action<SdkTheme> onTwoStep;
        Action onCancel;

        public void Show(
            string INtitle,
            Action<SdkTheme> INonPreferredOneStep = null,
            Action<SdkTheme> INonPreferredTwoStep = null,
            Action<SdkTheme> INonTwoStep = null,
            Action onCancel = null
            )
        {
            title.text = INtitle;
            onPreferredOneStep = INonPreferredOneStep;
            onPreferredTwoStep = INonPreferredTwoStep;
            onTwoStep = INonTwoStep;
            onCancel = onCancel != null ? onCancel : () => { gameObject.SetActive(false); };
            gameObject.SetActive(true);
        }

        public void PreferredOneStep()
        {
            gameObject.SetActive(false);
            var sdkTheme = GetSdkTheme();
            onPreferredOneStep?.Invoke(sdkTheme);
        }

        public void PreferredTwoStep()
        {
            gameObject.SetActive(false);
            var sdkTheme = GetSdkTheme();
            onPreferredTwoStep?.Invoke(sdkTheme);
        }

        public void TwoStep()
        {
            gameObject.SetActive(false);
            var sdkTheme = GetSdkTheme();
            onTwoStep?.Invoke(sdkTheme);
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
            onCancel?.Invoke();
        }

        private SdkTheme GetSdkTheme()
        {
            if (theme == null)
                return SdkTheme.LIGHT;

            return (SdkTheme)theme.value;
        }
    }
}
