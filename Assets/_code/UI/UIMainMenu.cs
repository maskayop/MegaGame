using UnityEngine;

namespace MegaGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public bool isOpen = false;

        [SerializeField] GameObject window;

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
            window.SetActive(true);
            CameraController.Instance.SetFarClipPlaneToZero(true);
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);
            CameraController.Instance.SetFarClipPlaneToZero(false);
        }

        public void OpenSettingsWindow()
        {
            UISettingsWindow.Instance.Open();
        }

        public void StartGame()
        {
            Close();
            GameController.Instance.StartGame();
        }
    }
}
