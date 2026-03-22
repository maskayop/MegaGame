using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class Tutorial : MonoBehaviour
    {
        public static Tutorial Instance { get; private set; }

        [SerializeField] DestroyAfterTime destroyAfterTime;

        [Header("Info")]
        public int currentProgress = 0;

        GameDataSaver gameDataSaver;

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
