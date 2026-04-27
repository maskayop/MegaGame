using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class UIBaseSwitchButton : MonoBehaviour
    {
        [Header("Visual")]
        public GameObject imageOn;
        public GameObject imageOff;

        protected Button button;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            button = GetComponent<Button>();

            OnInit();
        }

        protected virtual void OnInit() { }

        public virtual void Select(bool state)
        {
            imageOn.SetActive(state);
            imageOff.SetActive(!state);

            OnSelect();
        }

        protected virtual void OnSelect() { }
    }
}
