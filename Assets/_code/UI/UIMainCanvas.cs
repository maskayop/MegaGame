using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIMainCanvas : MonoBehaviour
    {
        public static UIMainCanvas Instance { get; private set; }

        [Header("Account")]
        [SerializeField] TextMeshProUGUI currentAccountNameText;

        [Header("Main Menu")]
        [SerializeField] UIMainMenu mainMenu;

        [Header("Game")]
        [SerializeField] GameObject startGameWindow;
        [SerializeField] GameObject endGameWindow;
        [SerializeField] GameObject victoryPanel;
        [SerializeField] GameObject defeatPanel;
        [SerializeField] GameObject endCampaignWindow;

        [Header("Clock")]
        [SerializeField] TextMeshProUGUI currentDayText;
        [SerializeField] Image clockFill;

        [Header("Wind")]
        [SerializeField] RectTransform windArrow;
        [SerializeField] Image windStrengthFillLeft;
        [SerializeField] Image windStrengthFillRight;

        [Header("Camera")]
        [SerializeField] float cameraZoomMultiplier = 2.0f;

        int currentDay = 0;

        GlobalTimeController globalTime;
        GameController gameController;

        GameController.GameState currentGameState;

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

            ShowStartGameWindow();
            mainMenu.Open();

            currentAccountNameText.text = gameController.GetAccountName();
        }

        void Update()
        {
            if (!gameController)
                return;

            UpdateClockAndWind();

            if (gameController.gameState != currentGameState)
            {
                if (gameController.gameState == GameController.GameState.battle)
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

        public void ShowStartGameWindow()
        {
            startGameWindow.SetActive(true);
            HideEndGameWindow();
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
        }

        public void PrepareNewBattle()
        {
            ShowStartGameWindow();
            gameController.PrepareNewBattle();
        }

        public void ShowEndCampaignWindow()
        {
            endCampaignWindow.SetActive(true);
        }
    }
}
