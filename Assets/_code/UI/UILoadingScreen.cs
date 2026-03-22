using UnityEngine;
using Vopere.Common;

namespace MegaGame.UI
{
    public class UILoadingScreen : MonoBehaviour
    {
        public static UILoadingScreen Instance { get; private set; }

        [SerializeField] GameObject warningPanel;

        [Header("Destroy")]
        [SerializeField] DestroyAfterTime destroyAfterTime;
        [SerializeField] float editorTimeToDestroy = 1;

        GameDataSaver gameDataSaver;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UILoadingScreen");
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

            if (!gameDataSaver)
                return;

            gameDataSaver.LoadTutorialData();

            if (gameDataSaver.GetCurrentTutorialState() == 0)
                warningPanel.SetActive(true);
            else
            {
                warningPanel.SetActive(false);

#if UNITY_EDITOR
                destroyAfterTime.DestroyGameObjectAfterTime(editorTimeToDestroy);
#else
                destroyAfterTime.DestroyGameObject();
#endif
            }
        }

        public void StartGame()
        {
            destroyAfterTime.DestroyGameObjectAfterTime(0);
            gameDataSaver.SaveTutorialData(1);
        }

        public void ExitGame()
        {
            App.Instance.ExitGame();
        }
    }
}
