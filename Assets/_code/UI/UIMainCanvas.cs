using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MegaGame.UI
{
    public class UIMainCanvas : MonoBehaviour
    {
        [Header("Game")]
        [SerializeField] GameObject startGameWindow;
        [SerializeField] GameObject endGameWindow;
        [SerializeField] GameObject victoryPanel;
        [SerializeField] GameObject defeatPanel;

        [Header("Clock")]
        [SerializeField] TextMeshProUGUI currentDayText;
        [SerializeField] Image clockFill;

        [Header("Wind")]
        [SerializeField] RectTransform windArrow;
        [SerializeField] Image windStrengthFillLeft;
        [SerializeField] Image windStrengthFillRight;

        [Header("Camera")]
        [SerializeField] float cameraZoomMultiplier = 2.0f;

        [Header("Money")]
        [SerializeField] TextMeshProUGUI playerMoneyAmounText;
        [SerializeField] TextMeshProUGUI enemyMoneyAmounText;        

        int currentDay = 0;

        GlobalTimeController globalTime;
        bool isBattle = false;

        void Start()
        {
            globalTime = GlobalTimeController.Instance;
            currentDayText.text = currentDay.ToString();

            ShowStartGameWindow();
        }

        void Update()
        {
            UpdateClockAndWind();
            UpdateMoney();

            if (GameController.Instance.IsBattle != isBattle)
            {
                if (GameController.Instance.IsBattle)
                    HideStartGameWindow();
                else
                    ShowEndGameWindow();
            }

            isBattle = GameController.Instance.IsBattle;
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

        void UpdateMoney()
        {
            playerMoneyAmounText.text = GameController.Instance.playerPiastres.ToString();
            enemyMoneyAmounText.text = GameController.Instance.enemyPiastres.ToString();
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

        void ShowStartGameWindow()
        {
            startGameWindow.SetActive(true);
            endGameWindow.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }

        void HideStartGameWindow()
        {
            startGameWindow.SetActive(false);
            endGameWindow.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }

        void ShowEndGameWindow()
        {
            startGameWindow.SetActive(false);
            endGameWindow.SetActive(true);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            if (GameController.Instance.IsVictory)
                victoryPanel.SetActive(true);
            else
                defeatPanel.SetActive(true);
        }

        public void StartGame()
        {
            HideStartGameWindow();
            GameController.Instance.StartBattle();
        }
    }
}
