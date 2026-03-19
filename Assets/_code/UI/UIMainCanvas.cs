using TMPro;
using UnityEngine;
using static MegaGame.GameController;

namespace MegaGame.UI
{
    public class UIMainCanvas : MonoBehaviour
    {
        public static UIMainCanvas Instance { get; private set; }

        [Header("Account")]
        [SerializeField] TextMeshProUGUI currentAccountNameText;

        [Header("Main Menu")]
        [SerializeField] UIMainMenu mainMenu;
        [SerializeField] GameObject goToMainMenuQuestionWindow;

        [Header("Game")]
        [SerializeField] GameObject startGameWindow;
        [SerializeField] GameObject endGameWindow;
        [SerializeField] GameObject victoryPanel;
        [SerializeField] GameObject defeatPanel;

        [Header("Campaign")]
        [SerializeField] GameObject endCampaignWindow;
        [SerializeField] GameObject campaignVictoryPanel;
        [SerializeField] GameObject campaignDefeatPanel;

        [Header("HUD")]
        [SerializeField] float cameraZoomMultiplier = 2.0f;
        [SerializeField] GameObject cameraZoomButtons;

        [SerializeField] UIExploringButton exploringButton;

        [Header("Messages")]
        [SerializeField] UIMessagePanel messagePanel;

        GameController gameController;
        GameDataSaver gameDataSaver;
        AdditionalSceneObjects additionalObjects;
        UISettingsWindow settingsWindow;
        UIGameShop gameShopWindow;

        GameState currentGameState;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIMainCanvas");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            gameController = GameController.Instance;
            gameDataSaver = GameDataSaver.Instance;
            additionalObjects = AdditionalSceneObjects.Instance;
            settingsWindow = UISettingsWindow.Instance;
            gameShopWindow = UIGameShop.Instance;

            ShowStartGameWindow();
        }

        void Update()
        {
            if (!gameController)
                return;

            currentAccountNameText.text = gameDataSaver.GetCurrentAccountName();

            if (gameController.CampaignIsEnded)
                return;

            if (mainMenu.isOpen)
            {
                currentGameState = GameState.menu;
                return;
            }

            if (gameController.gameState != currentGameState && currentGameState != GameState.menu)
            {
                if (gameController.gameState == GameState.battle)
                    HideStartGameWindow();
                else
                    ShowEndGameWindow();
            }
            else if (gameController.gameState != currentGameState && currentGameState == GameState.menu)
                ShowHUDButtons(false);

            currentGameState = gameController.gameState;
        }



        public void GoToCamera(bool isNext)
        {
            CameraController.Instance.GoToCamera(isNext);
        }

        public void CameraZoom(bool isCloser)
        {
            if (isCloser)
                CameraController.Instance.CameraZoom(-cameraZoomMultiplier);
            else
                CameraController.Instance.CameraZoom(+cameraZoomMultiplier);
        }

        public void ShowHUDButtons(bool state)
        {
            cameraZoomButtons.SetActive(state);
            exploringButton.Select(false);
            ResourcesController.Instance.CloseEnemyResources();
        }

        public void ShowStartGameWindow()
        {
            startGameWindow.SetActive(true);
            HideEndGameWindow();
            ShowHUDButtons(false);
        }

        void HideStartGameWindow()
        {
            startGameWindow.SetActive(false);
            HideEndGameWindow();
        }

        void ShowEndGameWindow()
        {
            startGameWindow.SetActive(false);
            endGameWindow.SetActive(true);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            additionalObjects.HideAllPanels();

            if (gameController.IsVictory)
            {
                victoryPanel.SetActive(true);
                additionalObjects.ShowVictoryPanel(true);
            }
            else
            {
                defeatPanel.SetActive(true);
                additionalObjects.ShowDefeatPanel(true);
            }

            ShowHUDButtons(false);
            HideSettingsWindow();
            HideShopWindow();
        }

        void HideEndGameWindow()
        {
            endGameWindow.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            additionalObjects.HideAllPanels();
        }

        public void StartBattle()
        {
            HideStartGameWindow();
            gameController.StartBattle();
            ShowHUDButtons(true);
            additionalObjects.HideAllPanels();
            UIShipsSelection.Instance.UpdateButtonsState();
        }

        public void PrepareNewBattle()
        {
            ShowStartGameWindow();
            gameController.PrepareNewBattle();
            ShowHUDButtons(false);
            additionalObjects.HideAllPanels();
        }

        public void ShowEndCampaignWindow()
        {
            endCampaignWindow.SetActive(true);
            campaignVictoryPanel.SetActive(false);
            campaignDefeatPanel.SetActive(false);

            if (gameController.CampaignIsEnded)
            {
                if (gameController.IsVictory)
                    campaignVictoryPanel.SetActive(true);
                else
                    campaignDefeatPanel.SetActive(true);
            }

            ShowHUDButtons(false);
            HideSettingsWindow();
            HideShopWindow();
        }

        void HideEndCampaignWindow()
        {
            endCampaignWindow.SetActive(false);
            campaignVictoryPanel.SetActive(false);
            campaignDefeatPanel.SetActive(false);
        }

        public void OpenMainMenu()
        {
            mainMenu.Open();
            HideEndCampaignWindow();
            CloseGoToMainMenuQuestionWindow();
            additionalObjects.HideAllPanels();
        }

        public void SpawnTooFarFromPortMessage()
        {
            messagePanel.SpawnTooFarFromPortMessage();
        }

        public void SpawnWrongTargetPortMessage(Island rightTarget)
        {
            messagePanel.SpawnWrongTargetPortMessage(rightTarget);
        }

        public void SpawnNekarkMessage()
        {
            messagePanel.SpawnNekarkMessage();
        }

        public void SpawnNafaivelMessage()
        {
            messagePanel.SpawnNafaivelMessage();
        }

        public void SpawnFortConstructionMessage(Port port)
        {
            messagePanel.SpawnFortConstructionMessage(port);
        }

        public void SpawnTraderConstructionMessage(Port port)
        {
            messagePanel.SpawnTraderConstructionMessage(port);
        }

        public void SpawnPiratesAttackVillageMessage(Village village)
        {
            messagePanel.SpawnPiratesAttackVillageMessage(village);
        }

        public void PlaceCamera()
        {
            if (gameController.gameState == GameState.menu)
                return;

            if (GameInputsController.Instance)
                GameInputsController.Instance.PlaceCamera();
        }

        public void OpenGoToMainMenuQuestionWindow()
        {
            goToMainMenuQuestionWindow.SetActive(true);
        }

        public void CloseGoToMainMenuQuestionWindow()
        {
            goToMainMenuQuestionWindow.SetActive(false);
        }

        void HideSettingsWindow()
        {
            if (settingsWindow)
            {
                if (settingsWindow.IsOpen)
                    settingsWindow.Close();
            }
        }

        void HideShopWindow()
        {
            if (gameShopWindow)
            {
                if (gameShopWindow.IsOpen)
                    gameShopWindow.Close();
            }
        }

        public void OpenShopWindow()
        {
            gameShopWindow.Open();
        }

        public void CloseShopWindow()
        {
            gameShopWindow.Close();
        }
    }
}
