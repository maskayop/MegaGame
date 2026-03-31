using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Tutorial : MonoBehaviour
    {
        public static Tutorial Instance { get; private set; }

        [Header("Info")]
        [SerializeField] List<Data_Tutorial> tutorialDataset = new List<Data_Tutorial>();

        public bool isTutorial = false;
        public bool IsTutorial { get { return isTutorial; } }

        GameDataSaver gameDataSaver;
        UITutorialWindow tutorialWindow;

        public int currentChapter = 0;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create Tutorial");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            gameDataSaver = GameDataSaver.Instance;
            tutorialWindow = UITutorialWindow.Instance;

            gameDataSaver.LoadTutorialData();
            currentChapter = gameDataSaver.GetCurrentTutorialState();

            CheckForTutorialState();
        }

        public void EndTutorial()
        {
            gameDataSaver.SaveTutorialData(31);
            isTutorial = false;
        }

        public void ShowTutorialChapter(int id)
        {
            currentChapter = id;
            tutorialWindow.Open();

            CheckForTutorialState();

            if (isTutorial)
                tutorialWindow.ShowTutorialChapter(tutorialDataset[id - 1]);
        }

        public void ShowNextChapter()
        {
            currentChapter++;

            if (currentChapter > tutorialDataset.Count)
                tutorialWindow.SkipTutorial();
            else
                ShowTutorialChapter(currentChapter);
        }

        void CheckForTutorialState()
        {
            if (currentChapter == 31)
            {
                isTutorial = false;
                return;
            }
            else
                isTutorial = true;
        }
    }
}
