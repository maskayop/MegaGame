using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UITutorialWindow : MonoBehaviour
    {
        public static UITutorialWindow Instance { get; private set; }

        [Header("Background")]
        [SerializeField] GameObject background;
        [SerializeField] GameObject backgroundTarget;

        [Header("Panel")]
        [SerializeField] Animator assistantAnimator;
        [SerializeField] GameObject tutorialPanel;
        [SerializeField] Animator tutorialPanelAnimator;
        [SerializeField] Image readyStatusFillImage;
        [SerializeField] int readyStatusCountdown;

        [Header("Texts")]
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] TextMeshProUGUI descriptionText;

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

        float currentTime = 0;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UITutorialWindow");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        void Update()
        {

        }

        public void Init()
        {
            Close();
        }

        public void Open()
        {
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }
    }
}
