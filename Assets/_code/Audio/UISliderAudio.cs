using UnityEngine;
using UnityEngine.UI;

namespace MegaGame
{
    [RequireComponent(typeof(Slider))]
    public class UISliderAudio : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        Slider slider;

        void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        void OnSliderChanged(float value)
        {
            AudioController.Instance.PlayUIAudioClip(clip);
        }

        void OnDestroy()
        {
            if (slider != null)
                slider.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }
}
