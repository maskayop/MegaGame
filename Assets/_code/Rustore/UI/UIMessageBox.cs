using System;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessageBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] TextMeshProUGUI messageText;

        Action onCloseAction;

        public void Show(string title = null, string message = null, Action onClose = null)
        {
            if (titleText != null && title != null) titleText.text = title;
            if (messageText != null && message != null) messageText.text = message;

            onCloseAction = onClose;

            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            onCloseAction?.Invoke();
        }
    }
}
