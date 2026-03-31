using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UITutorialWindow : MonoBehaviour
    {
        public static UITutorialWindow Instance { get; private set; }

        [SerializeField] GameObject window;

        [Header("Background")]
        [SerializeField] GameObject background;
        [SerializeField] GameObject backgroundTarget;
        [SerializeField] GameObject targetCircles;

        [Header("Panel")]
        [SerializeField] Animator assistantAnimator;
        [SerializeField] GameObject tutorialPanel;
        [SerializeField] Animator tutorialPanelAnimator;
        [SerializeField] Image readyStatusFillImage;
        [SerializeField] int readyStatusCountdown = 1;

        [Header("Texts")]
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] TextMeshProUGUI readyButtonText;

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

        Tutorial tutorial;
        CameraController cameraController;
        AdditionalSceneObjects sceneObjects;

        float currentTime = 0;
        bool waitForUser = false;

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
            if (waitForUser)
                return;

            if (!isOpen)
                return;

            currentTime -= Time.deltaTime;

            if (currentTime < 0)
                ShowNextChapter();

            readyStatusFillImage.fillAmount = 1.0f - (float)(currentTime / readyStatusCountdown);
        }

        public void Init()
        {
            tutorial = Tutorial.Instance;
            cameraController = CameraController.Instance;
            sceneObjects = AdditionalSceneObjects.Instance;

            Close();
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);
        }

        public void SkipTutorial()
        {
            tutorial?.EndTutorial();
            Close();
            cameraController.TutorialFreeze(false);
            sceneObjects.ShowStartGameModelButton(true);
        }

        public void ShowNextChapter()
        {
            tutorialPanel.SetActive(false);
            assistantAnimator.gameObject.SetActive(false);

            tutorial?.ShowNextChapter();
        }

        public void ShowTutorialChapter(Data_Tutorial data)
        {
            tutorialPanel.SetActive(true);
            assistantAnimator.gameObject.SetActive(true);

            if (data.showTarget)
            {
                background.SetActive(false);
                backgroundTarget.SetActive(true);
                targetCircles.SetActive(true);
            }
            else
            {
                background.SetActive(true);
                backgroundTarget.SetActive(false);
                targetCircles.SetActive(false);
            }

            cameraController.TutorialFreeze(data.freezeCamera);

            sceneObjects.ShowStartGameModelButton(data.showStartBattleMedal);

            waitForUser = data.waitForUser;
            currentTime = readyStatusCountdown;

            titleText.text = data.title.GetLocalizedString();
            descriptionText.text = data.description.GetLocalizedString();
            readyButtonText.text = data.readyButtonText.GetLocalizedString();
        }
    }
}
