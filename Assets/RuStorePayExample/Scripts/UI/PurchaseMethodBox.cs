using RuStore.PayClient;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuStore.PayExample.UI {

    public class PurchaseMethodBox : MonoBehaviour {

        [SerializeField]
        private Text _title;

        [SerializeField]
        private Dropdown _theme;

        private Action<SdkTheme> _onPreferredOneStep;
        private Action<SdkTheme> _onPreferredTwoStep;
        private Action<SdkTheme> _onTwoStep;
        private Action _onCancel;

        public void Show(string title, Action<SdkTheme> onPreferredOneStep = null, Action<SdkTheme> onPreferredTwoStep = null, Action<SdkTheme> onTwoStep = null, Action onCancel = null) {
            _title.text = title;
            _onPreferredOneStep = onPreferredOneStep;
            _onPreferredTwoStep = onPreferredTwoStep;
            _onTwoStep = onTwoStep;
            _onCancel = onCancel != null ? onCancel : () => { gameObject.SetActive(false); };
            gameObject.SetActive(true);
        }

        public void PreferredOneStep() {
            gameObject.SetActive(false);
            var sdkTheme = GetSdkTheme();
            _onPreferredOneStep?.Invoke(sdkTheme);
        }

        public void PreferredTwoStep() {
            gameObject.SetActive(false);
            var sdkTheme = GetSdkTheme();
            _onPreferredTwoStep?.Invoke(sdkTheme);
        }

        public void TwoStep() {
            gameObject.SetActive(false);
            var sdkTheme = GetSdkTheme();
            _onTwoStep?.Invoke(sdkTheme);
        }

        public void Cancel() {
            gameObject.SetActive(false);
            _onCancel?.Invoke();
        }

        private SdkTheme GetSdkTheme() {
            if (_theme == null) return SdkTheme.LIGHT;

            return (SdkTheme)_theme.value;
        }
    }
}
