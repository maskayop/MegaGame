using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public bool isOpen = false;

        [Header("Windows")]
        [SerializeField] GameObject mainWindow;
        [SerializeField] GameObject accountManagerWindow;

        [Header("Account")]
        [SerializeField] TextMeshProUGUI currentAccountNameText;
        [SerializeField] TMP_InputField accountNameInputField;
        [SerializeField] GameObject renameAccountButton;

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (!isOpen)
                return;

            if (accountNameInputField.text == GameController.Instance.GetAccountName())
                renameAccountButton.SetActive(false);
            else
                renameAccountButton.SetActive(true);
        }

        public void Init()
        {
            currentAccountNameText.text = GameController.Instance.GetAccountName();
            CloseAccountManagerWindow();
        }

        public void Open()
        {
            isOpen = true;
            mainWindow.SetActive(true);
            CameraController.Instance.SetFarClipPlaneToZero(true);
        }

        public void Close()
        {
            isOpen = false;
            mainWindow.SetActive(false);
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

        public void OpenAccountManagerWindow()
        {
            accountManagerWindow.SetActive(true);
            accountNameInputField.text = GameController.Instance.GetAccountName();
        }

        public void CloseAccountManagerWindow()
        {
            accountManagerWindow.SetActive(false);
        }

        public void OnRenameAccountButtonClicked()
        {
            GameController.Instance.SetAccountName(accountNameInputField.text);
            Init();
        }
    }
}
