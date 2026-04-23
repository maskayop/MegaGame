using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame
{
    [RequireComponent(typeof(Slider))]
    public class UISliderAudio : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        Slider slider;
        bool firstClick = true;

        void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnSliderChanged);

            StartCoroutine(InitDelayed());
        }

        IEnumerator InitDelayed()
        {
            yield return new WaitForSeconds(1f);
            firstClick = false;
        }

        void OnSliderChanged(float value)
        {
            if (firstClick)
            {
                firstClick = false;
                return;
            }

            AudioController.Instance.PlayUIAudioClip(clip);
        }

        void OnDestroy()
        {
            if (slider != null)
                slider.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }
}
