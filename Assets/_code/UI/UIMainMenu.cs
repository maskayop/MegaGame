using TMPro;
using UnityEngine;
using Vopere.Common;

namespace MegaGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public static UIMainMenu Instance { get; private set; }

        public bool isOpen = false;

        [Header("Windows")]
        [SerializeField] GameObject mainWindow;
        [SerializeField] GameObject menuButtons;
        [SerializeField] GameObject exitAppWindow;

        [Header("Account")]
        [SerializeField] TextMeshProUGUI currentAccountNameText;

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
        UIAccountManagerWindow accountManagerWindow;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIMainMenu");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
            Open();
        }

        void Update()
        {
            if (!isOpen)
                return;
        }

        public void Init()
        {
            gameController = GameController.Instance;
            gameDataSaver = GameDataSaver.Instance;
            cameraController = CameraController.Instance;
            gameShop = GameShop.Instance;
            gameShopUI = UIGameShop.Instance;
            globalTime = GlobalTimeController.Instance;
            accountManagerWindow = UIAccountManagerWindow.Instance;

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
            accountManagerWindow.Open();
        }

        public void CloseAccountManagerWindow()
        {
            accountManagerWindow.Close();
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
