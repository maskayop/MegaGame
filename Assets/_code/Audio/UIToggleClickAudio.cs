using UnityEngine;
using UnityEngine.UI;

namespace MegaGame
{
    [RequireComponent(typeof(Toggle))]
    public class UIToggleClickAudio : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        Toggle toggle;

        void Start()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnToggleClick);
        }

        void OnToggleClick(bool isOn)
        {
            if (isOn)
                AudioController.Instance.PlayUIAudioClip(clip);
        }

        void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveListener(OnToggleClick);
        }
    }
}
