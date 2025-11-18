using UnityEngine;
using UnityEngine.UI;

namespace MegaGame
{
    [RequireComponent(typeof(Button))]
    public class UIButtonClickAudio : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        Button button;

        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            if (AudioController.Instance)
                AudioController.Instance.PlayUIAudioClip(clip);
        }

        void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
