using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    [RequireComponent(typeof(Button))]
    public class UISendSpiesButton : MonoBehaviour
    {
        [SerializeField] GameObject imageOff;
        [SerializeField] GameObject imageOn;

        Button button;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            button = GetComponent<Button>();

            Select(false);
        }

        public void Select(bool state)
        {
            imageOn.SetActive(state);
            imageOff.SetActive(!state);
            button.interactable = !state;
        }
    }
}
