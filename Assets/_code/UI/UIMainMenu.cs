using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MegaGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public bool isOpen = false;

        void Start()
        {
            Init();
        }

        void Update()
        {

        }

        public void Init()
        {

        }

        public void Open()
        {
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }

        public void OpenSettingsWindow()
        {
            UISettingsWindow.Instance.Open();
        }
    }
}
