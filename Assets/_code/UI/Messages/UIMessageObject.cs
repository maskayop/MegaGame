using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIMessageObject : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI text;

        public void SetText(string t)
        {
            if (!text)
                return;

            text.text = t;
        }

        public void SetImageColor(Color c)
        {
            if (!image)
                return;

            image.color = c;
        }
    }
}
