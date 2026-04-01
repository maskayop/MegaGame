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

        [Header("Main Panel")]
        [SerializeField] Animator assistantAnimator;
        [SerializeField] GameObject tutorialPanel;
        [SerializeField] Animator tutorialPanelAnimator;
        [SerializeField] Image readyStatusFillImage;
        [SerializeField] int readyStatusCountdown = 1;
        [SerializeField] GameObject readyButton;

        [Header("Texts")]
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] TextMeshProUGUI readyButtonText;

        [Header("Skip Window")]
        [SerializeField] GameObject skipQuestionWindow;

        bool skipQuestionWindowIsOpen = false;

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

        Tutorial tutorial;
        CameraController cameraController;
        AdditionalSceneObjects sceneObjects;
        GameController gameController;
        ObjectsManager objectsManager;
        GlobalTimeController globalTime;

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
            {
                if (tutorial.currentChapter == 5)
                {
                    if (objectsManager.playerShips.Count == 1)
                    {
                        ShowNextChapter();
                        cameraController.SetPosition(objectsManager.playerShips[0].transform.position);
                        cameraController.SetTranslationZToBase();
                    }
                }

                readyStatusFillImage.fillAmount = 0;
                return;
            }

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
            gameController = GameController.Instance;
            objectsManager = ObjectsManager.Instance;
            globalTime = GlobalTimeController.Instance;

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

        public void OpenSkipQuestionWindow()
        {
            skipQuestionWindowIsOpen = true;
            skipQuestionWindow.SetActive(true);
        }

        public void CloseSkipQuestionWindow()
        {
            skipQuestionWindowIsOpen = false;
            skipQuestionWindow.SetActive(false);
        }

        public void SkipTutorial()
        {
            tutorial?.EndTutorial();
            cameraController.TutorialFreeze(false);
            sceneObjects.ShowStartGameModelButton(true);

            Close();

            if (skipQuestionWindowIsOpen)
                CloseSkipQuestionWindow();
        }

        public void ShowNextChapter()
        {
            tutorialPanel.SetActive(false);
            assistantAnimator.gameObject.SetActive(false);

            tutorial?.ShowNextChapter();

            if (skipQuestionWindowIsOpen)
                CloseSkipQuestionWindow();
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

            if (data.hideBackgrounds)
            {
                background.SetActive(false);
                backgroundTarget.SetActive(false);
                targetCircles.SetActive(false);
            }

            cameraController.TutorialFreeze(data.freezeCamera);
            globalTime.FreezeTime(data.freezeTime);

            sceneObjects.ShowStartGameModelButton(data.showStartBattleMedal);

            waitForUser = data.waitForUser;

            if (waitForUser)
                readyButton.SetActive(false);
            else
                readyButton.SetActive(true);

            currentTime = readyStatusCountdown;

            titleText.text = data.title.GetLocalizedString();
            descriptionText.text = data.description.GetLocalizedString();
            readyButtonText.text = data.readyButtonText.GetLocalizedString();

            ShowCameraTarget();
            cameraController.SetTranslationZToMax();
        }

        void ShowCameraTarget()
        {
            if (!cameraController || !gameController || !tutorial)
                return;

            if (tutorial.currentChapter == 2)
                cameraController.SetPosition(gameController.playerOpposingPorts.protagonPort.transform.position);
            else if (tutorial.currentChapter == 3)
                cameraController.SetPosition(gameController.playerOpposingPorts.antagonPort.transform.position);
            else if (tutorial.currentChapter == 4)
                cameraController.SetPosition(sceneObjects.GetStartGameMedal().transform.position);
            else if (tutorial.currentChapter == 5)
                cameraController.SetPosition(gameController.playerOpposingPorts.antagonPort.transform.position);
        }
    }
}
