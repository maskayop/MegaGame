using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIShopLoadingIndicator : MonoBehaviour
    {
        [SerializeField] RectTransform indicator;
        [SerializeField] Image indicatorFillImage;
        [SerializeField] float rotationSpeed;
        [SerializeField] float fillSpeed;
        [SerializeField] float fillStart;

        float currentFillSpeed;

        void Awake()
        {
            ResetIndicator();
        }

        void Start()
        {
            Hide();
        }

        void Update()
        {
            var fill = indicatorFillImage.fillAmount + currentFillSpeed * Time.deltaTime;

            if (fill < 0f || fill > 1f)
                currentFillSpeed = -currentFillSpeed;

            indicatorFillImage.fillAmount = fill;
            indicator.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.deltaTime));
        }

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                ResetIndicator();
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void ResetIndicator()
        {
            currentFillSpeed = fillSpeed;
            indicatorFillImage.fillAmount = fillStart;
        }
    }
}
