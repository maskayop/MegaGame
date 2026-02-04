using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public bool isOpen = false;

        [Header("Windows")]
        [SerializeField] GameObject mainWindow;

        [Header("Accounts")]
        [SerializeField] GameObject accountManagerWindow;
        [SerializeField] GameObject accountButtonPrefab;
        [SerializeField] Transform accountButtonsContainer;

        [Header("Account")]
        [SerializeField] TextMeshProUGUI currentAccountNameText;
        [SerializeField] TMP_InputField accountNameInputField;
        [SerializeField] GameObject renameAccountButton;
        [SerializeField] GameObject createAccountButton;

        GameController gameController;

        List<string> accountsNames = new List<string>();

        void Start()
        {
            Init();
            Open();
        }

        void Update()
        {
            if (!isOpen)
                return;

            if (accountNameInputField.text == gameController.GetCurrentAccountName())
            {
                renameAccountButton.SetActive(false);
                createAccountButton.SetActive(false);
            }
            else
            {
                renameAccountButton.SetActive(true);
                createAccountButton.SetActive(true);
            }
        }

        public void Init()
        {
            gameController = GameController.Instance;

            currentAccountNameText.text = gameController.GetCurrentAccountName();
            CloseAccountManagerWindow();
        }

        public void Open()
        {
            isOpen = true;
            mainWindow.SetActive(true);
            CameraController.Instance.SetFarClipPlaneToZero(true);
            gameController.SetGameStateAsMenu();
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
            gameController.StartGame();
        }

        public void OpenAccountManagerWindow()
        {
            accountManagerWindow.SetActive(true);
            accountNameInputField.text = gameController.GetCurrentAccountName();
            GetAccountsNames();
        }

        public void CloseAccountManagerWindow()
        {
            accountManagerWindow.SetActive(false);
        }

        public void OnRenameAccountButtonClicked()
        {
            gameController.SetAccountName(accountNameInputField.text);
            Init();
        }

        public void OnCreateAccountButtonClicked()
        {
            gameController.CreateAccount(accountNameInputField.text);
            Init();
        }

        void GetAccountsNames()
        {
            foreach (Transform t in accountButtonsContainer)
                Destroy(t.gameObject);

            accountsNames = gameController.GetAccountsNames();

            for (int i = 0; i < accountsNames.Count; i++)
            {
                GameObject b = Instantiate(accountButtonPrefab, accountButtonsContainer);
                b.GetComponent<UILoadAccountButton>().Init(this, accountsNames[i]);
            }
        }

        public void LoadAccount(string targetAccountName)
        {
            GameController.Instance.LoadAccount(targetAccountName);
            gameController.SetAccountName(targetAccountName);
            Init();
        }
    }
}
