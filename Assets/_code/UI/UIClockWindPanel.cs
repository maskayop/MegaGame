using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIClockWindPanel : MonoBehaviour
    {
        [Header("Clock")]
        [SerializeField] TextMeshProUGUI currentDayText;
        [SerializeField] Image clockFill;

        [Header("Wind")]
        [SerializeField] RectTransform windArrow;
        [SerializeField] Image windStrengthFillLeft;
        [SerializeField] Image windStrengthFillRight;

        GlobalTimeController globalTime;
        GameController gameController;

        int currentDay = 0;

        void Start()
        {
            globalTime = GlobalTimeController.Instance;
            gameController = GameController.Instance;

            currentDayText.text = currentDay.ToString();
        }

        void Update()
        {
            if (!gameController || !globalTime)
                return;

            if (gameController.CampaignIsEnded)
                return;

            UpdateClockAndWind();
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

        public void PlaceCamera()
        {
            if (UIMainCanvas.Instance)
                UIMainCanvas.Instance.PlaceCamera();
        }
    }
}
