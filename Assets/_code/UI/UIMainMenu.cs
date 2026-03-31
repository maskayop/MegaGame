using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vopere.Common;

namespace MegaGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public bool isOpen = false;

        [Header("Windows")]
        [SerializeField] GameObject mainWindow;
        [SerializeField] GameObject menuButtons;

        [SerializeField] GameObject exitAppWindow;

        [Header("Accounts")]
        [SerializeField] GameObject accountManagerWindow;
        [SerializeField] GameObject accountButtonPrefab;
        [SerializeField] Transform accountButtonsContainer;

        [Header("Account")]
        [SerializeField] TextMeshProUGUI currentAccountNameText;
        [SerializeField] TMP_InputField accountNameInputField;
        [SerializeField] GameObject renameAccountButton;
        [SerializeField] GameObject createAccountButton;

        [Header("No Ads")]
        [SerializeField] GameObject noAdsButton;
        [SerializeField] GameObject noAdsPanel;
        [SerializeField] int minDayForNoAdsButton = 100;

        GameController gameController;
        GameDataSaver gameDataSaver;
        CameraController cameraController;
        GlobalTimeController globalTime;
        GameShop gameShop;
        UIGameShop gameShopUI;

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

            if (accountNameInputField.text == gameDataSaver.GetCurrentAccountName())
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
            gameDataSaver = GameDataSaver.Instance;
            cameraController = CameraController.Instance;
            gameShop = GameShop.Instance;
            gameShopUI = UIGameShop.Instance;
            globalTime = GlobalTimeController.Instance;

            currentAccountNameText.text = gameDataSaver.GetCurrentAccountName();
            CloseAccountManagerWindow();

            Tutorial.Instance?.Init();
        }

        public void Open()
        {
            isOpen = true;
            mainWindow.SetActive(true);
            gameController.SetGameStateAsMenu();

            cameraController?.SetFarClipPlaneToZero(true);

            TryShowNoAdsButton();
        }

        public void Close()
        {
            isOpen = false;
            mainWindow.SetActive(false);

            cameraController?.SetFarClipPlaneToZero(false);
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
            accountNameInputField.text = gameDataSaver.GetCurrentAccountName();
            CreateAccountsButtons();
        }

        public void CloseAccountManagerWindow()
        {
            accountManagerWindow.SetActive(false);
        }

        public void OnRenameAccountButtonClicked()
        {
            gameDataSaver.SetAccountName(accountNameInputField.text);
            Init();
        }

        public void OnCreateAccountButtonClicked()
        {
            gameDataSaver.CreateAccount(accountNameInputField.text);
            Init();
        }

        void CreateAccountsButtons()
        {
            foreach (Transform t in accountButtonsContainer)
                Destroy(t.gameObject);

            accountsNames = gameDataSaver.GetAccountsNames();

            for (int i = 0; i < accountsNames.Count; i++)
            {
                GameObject b = Instantiate(accountButtonPrefab, accountButtonsContainer);
                b.GetComponent<UILoadAccountButton>().Init(this, accountsNames[i]);
            }
        }

        public void LoadAccount(string targetAccountName)
        {
            gameDataSaver.LoadAccount(targetAccountName);
            gameDataSaver.SetAccountName(targetAccountName);
            Init();
        }

        public void DeletePlayerPrefs()
        {
            DataSaveLoad.Instance.DeletePlayerPrefs();

            foreach (Transform t in accountButtonsContainer)
                Destroy(t.gameObject);

            gameDataSaver.LoadLastAccount();

            Init();
        }

        public void OpenCloseMenuButtons()
        {
            menuButtons.SetActive(!menuButtons.activeSelf);
        }

        public void Exit()
        {
            App.Instance.ExitGame();
        }

        public void OpenExitAppWindow()
        {
            exitAppWindow.SetActive(true);
        }

        public void CloseExitAppWindow()
        {
            exitAppWindow.SetActive(false);
        }

        public void OpenShopWindow()
        {
            gameShopUI.Open();
        }

        public void CloseShopWindow()
        {
            gameShopUI.Close();
            TryShowNoAdsButton();
        }

        void TryShowNoAdsButton()
        {
            if (globalTime.currentDay > minDayForNoAdsButton && !gameShop.CheckForAllPremiumItemPurchased())
                noAdsButton.SetActive(true);
            else
                noAdsButton.SetActive(false);
        }

        public void OpenNoAdsPanel()
        {
            noAdsPanel.SetActive(true);
            noAdsButton.SetActive(false);
        }

        public void CloseNoAdsPanel()
        {
            noAdsPanel.SetActive(false);
            noAdsButton.SetActive(true);
        }
    }
}
