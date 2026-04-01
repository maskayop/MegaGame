using System.Collections.Generic;
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
        [SerializeField] GameObject assistantRight;
        [SerializeField] GameObject assistantLeft;
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

        [Header("Additional Objects")]
        [SerializeField] List<GameObject> additionalObjects = new List<GameObject>();
        [SerializeField] List<RectTransform> additionalRectTransforms = new List<RectTransform>();

        bool skipQuestionWindowIsOpen = false;

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

        Tutorial tutorial;
        CameraController cameraController;
        AdditionalSceneObjects sceneObjects;
        GameController gameController;
        ObjectsManager objectsManager;
        GlobalTimeController globalTime;

        RectTransform targetCirclesRectTransform;

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
            if (!tutorial)
                return;

            if (!tutorial.isTutorial)
                return;

            if (waitForUser)
            {
                if (tutorial.currentChapter == 5)
                {
                    if (objectsManager.playerShips.Count == 1)
                        ShowNextChapter();
                }

                readyStatusFillImage.fillAmount = 0;
                return;
            }

            if (tutorial.currentChapter == 6 || tutorial.currentChapter == 7 || tutorial.currentChapter == 8)
                if (objectsManager.playerShips.Count != 0)
                    cameraController.SetPosition(objectsManager.playerShips[0].transform.position);

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

            targetCirclesRectTransform = targetCircles.GetComponent<RectTransform>();

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

            if (tutorial.currentChapter == 0 || tutorial.currentChapter == 1)
            {
                ShowOnlyObject(-1);
                assistantRight.SetActive(false);
                assistantLeft.SetActive(false);
            }
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
            assistantRight.SetActive(false);
            assistantLeft.SetActive(false);

            ShowOnlyObject(-1);
            tutorial?.ShowNextChapter();

            if (skipQuestionWindowIsOpen)
                CloseSkipQuestionWindow();
        }

        public void ShowTutorialChapter(Data_Tutorial data)
        {
            tutorialPanel.SetActive(true);

            if (data.assistantLeftPosition)
                assistantLeft.SetActive(true);
            else
                assistantRight.SetActive(true);

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

            cameraController.SetTranslationZToMax();
            ShowCameraTarget();
        }

        void ShowCameraTarget()
        {
            if (!cameraController || !gameController || !tutorial)
                return;

            if (tutorial.currentChapter == 1)
                cameraController.SetPosition(Vector3.zero);
            else if (tutorial.currentChapter == 2)
                cameraController.SetPosition(gameController.playerOpposingPorts.protagonPort.transform.position);
            else if (tutorial.currentChapter == 3)
                cameraController.SetPosition(gameController.playerOpposingPorts.antagonPort.transform.position);
            else if (tutorial.currentChapter == 4)
                cameraController.SetPosition(sceneObjects.GetStartGameMedal().transform.position);
            else if (tutorial.currentChapter == 5)
                cameraController.SetPosition(gameController.playerOpposingPorts.antagonPort.transform.position);
            else if (tutorial.currentChapter == 6)
            {
                cameraController.SetTranslationZToBase();
                ShowOnlyObject(2);
                SetTargetCirclesPosition(additionalRectTransforms[0]);
            }
            else if (tutorial.currentChapter == 7)
            {
                ShowOnlyObject(5);
                SetTargetCirclesPosition(additionalRectTransforms[1]);
            }
            else if (tutorial.currentChapter == 8)
            {
                ShowOnlyObject(-1);
                SetTargetCirclesPosition(GetComponent<RectTransform>());
            }
        }

        void ShowOnlyObject(int id)
        {
            foreach (GameObject g in additionalObjects)
                g.SetActive(false);

            if (id == -1)
                return;

            for (int i = 0; i < additionalObjects.Count; i++)
                if (i == id)
                    additionalObjects[i].SetActive(true);
        }

        void SetTargetCirclesPosition(RectTransform INtransform)
        {
            targetCirclesRectTransform.position = INtransform.position;
        }
    }
}
