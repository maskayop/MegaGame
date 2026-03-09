using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        [Header("Clock")]
        [SerializeField] TextMeshProUGUI currentDayText;
        [SerializeField] Image clockFill;

        [Header("Wind")]
        [SerializeField] RectTransform windArrow;
        [SerializeField] Image windStrengthFillLeft;
        [SerializeField] Image windStrengthFillRight;

        [Header("Camera")]
        [SerializeField] float cameraZoomMultiplier = 2.0f;
        [SerializeField] GameObject cameraZoomButtons;

        [Header("Messages")]
        [SerializeField] UIMessagePanel messagePanel;

        int currentDay = 0;

        GlobalTimeController globalTime;
        GameController gameController;
        GameDataSaver gameDataSaver;

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
            globalTime = GlobalTimeController.Instance;
            gameController = GameController.Instance;
            currentDayText.text = currentDay.ToString();
            gameDataSaver = GameDataSaver.Instance;

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
                currentGameState = gameController.gameState;
                return;
            }

            UpdateClockAndWind();

            if (gameController.gameState != currentGameState && currentGameState != GameState.menu)
            {
                if (gameController.gameState == GameState.battle)
                    HideStartGameWindow();
                else
                    ShowEndGameWindow();
            }

            currentGameState = gameController.gameState;
        }

        void UpdateClockAndWind()
        {
            if (globalTime.currentDay != currentDay)
            {
                currentDay = globalTime.currentDay;
                currentDayText.text = currentDay.ToString();
            }

            clockFill.fillAmount = globalTime.currentTime / globalTime.dayLenght;
            windArrow.rotation = Quaternion.Euler(0, 0, -WindController.Instance.currentRotation.eulerAngles.y);
            windStrengthFillLeft.fillAmount = windStrengthFillRight.fillAmount = WindController.Instance.GetNormalizedCurrentStrength() / 2;
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

        public void ShowCameraZoomButtons(bool state)
        {
            cameraZoomButtons.SetActive(state);
        }

        public void ShowStartGameWindow()
        {
            startGameWindow.SetActive(true);
            HideEndGameWindow();
            ShowCameraZoomButtons(false);
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

            if (gameController.IsVictory)
                victoryPanel.SetActive(true);
            else
                defeatPanel.SetActive(true);

            ShowCameraZoomButtons(false);
        }

        void HideEndGameWindow()
        {
            endGameWindow.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }

        public void StartBattle()
        {
            HideStartGameWindow();
            gameController.StartBattle();
            ShowCameraZoomButtons(true);
        }

        public void PrepareNewBattle()
        {
            ShowStartGameWindow();
            gameController.PrepareNewBattle();
            ShowCameraZoomButtons(false);
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

            ShowCameraZoomButtons(false);
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

        public void SpawnFortConstructionMessage(Port port)
        {
            messagePanel.SpawnFortConstructionMessage(port);
        }

        public void SpawnTraderConstructionMessage(Port port)
        {
            messagePanel.SpawnTraderConstructionMessage(port);
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
    }
}
