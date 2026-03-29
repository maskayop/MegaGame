using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIShopMessagePanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI messageText;

        public void Show(string message)
        {
            if (messageText != null && message != null) messageText.text = message;

            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
