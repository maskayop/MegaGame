using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessageObject : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        public void SetText(string t)
        {
            text.text = t;
        }
    }
}
