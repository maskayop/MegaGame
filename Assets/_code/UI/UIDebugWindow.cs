using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIDebugWindow : MonoBehaviour
    {
        public static UIDebugWindow Instance { get; private set; }

        [SerializeField] TextMeshProUGUI text;

        GameShop gameshop;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIDebugWindow");
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
            gameshop = GameShop.Instance;

            RustoreValidation();
        }

        public void SetText(string INtext)
        {
            text.text = INtext;
        }

        public void AddText(string INtext)
        {
            text.text += INtext;
        }

        public void RustoreValidation()
        {
            SetText("");

            for (int i = 0; i < gameshop.currentRustoreStatus.Length; i++)
                AddText(gameshop.currentRustoreStatus[i] + "\n");
        }
    }
}
