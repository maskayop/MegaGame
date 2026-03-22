using UnityEngine;
using Vopere.Common;

namespace MegaGame.UI
{
    public class UILoadingScreen : MonoBehaviour
    {
        public static UILoadingScreen Instance { get; private set; }

        [SerializeField] DestroyAfterTime destroyAfterTime;
        [SerializeField] GameObject warningPanel;

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
        }
    }
}
