using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class Tutorial : MonoBehaviour
    {
        public static Tutorial Instance { get; private set; }

        [Header("Info")]
        [SerializeField] List<Data_Tutorial> tutorialDataset = new List<Data_Tutorial>();

        [Header("Progress")]
        public int currentProgress = 0;

        GameDataSaver gameDataSaver;

        DestroyAfterTime destroyAfterTime;

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
            gameDataSaver.LoadTutorialData();
        }
    }
}
