using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame
{
    [RequireComponent(typeof(Toggle))]
    public class UIToggleClickAudio : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        Toggle toggle;
        bool firstClick = true;

        void Start()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnToggleClick);

            StartCoroutine(InitDelayed());
        }

        IEnumerator InitDelayed()
        {
            yield return new WaitForSeconds(1f);
            firstClick = false;
        }

        void OnToggleClick(bool isOn)
        {
            if (firstClick)
            {
                firstClick = false;
                return;
            }

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
