using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIRustoreDebugWindow : MonoBehaviour
    {
        public static UIRustoreDebugWindow Instance { get; private set; }

        [SerializeField] TextMeshProUGUI text;

        [SerializeField] GameObject productCardPrefab;
        [SerializeField] Transform productCardsContainer;

        [SerializeField] GameObject purchaseCardPrefab;
        [SerializeField] Transform purchasesCardsContainer;

        GameShop gameshop;
        RustorePayments rustorePayments;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIRustoreDebugWindow");
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
            rustorePayments = RustorePayments.Instance;

            RustoreValidation();
            CreateProductsCards();
            CreatePurchasesCards();
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

            for (int i = 0; i < rustorePayments.currentRustoreStatus.Length; i++)
                AddText(rustorePayments.currentRustoreStatus[i] + "\n");

            if (string.IsNullOrWhiteSpace(text.text))
                text.text = "No data";
        }

        public void TryLoadRustoreProducts()
        {
            RustorePayments.Instance.LoadProducts();
        }

        public void ReloadRustoreValidation()
        {
            if (RustorePayments.Instance)
                RustorePayments.Instance.Init();
        }

        public void CreateProductsCards()
        {
            foreach (Transform t in productCardsContainer)
                Destroy(t.gameObject);

            if (!gameshop)
                return;

            AddText("\n" + "products count = " + rustorePayments.products.Count.ToString());

            if (rustorePayments.products.Count == 0)
                return;

            for (int i = 0; i < rustorePayments.products.Count; i++)
            {
                GameObject go = Instantiate(productCardPrefab, productCardsContainer);

                go.GetComponent<UIProductCard>().SetData(rustorePayments.products[i]);
            }
        }

        public void CreatePurchasesCards()
        {
            foreach (Transform t in purchasesCardsContainer)
                Destroy(t.gameObject);

            if (!gameshop)
                return;

            AddText("\n" + "purchases count = " + rustorePayments.purchases.Count.ToString());

            if (rustorePayments.purchases.Count == 0)
                return;

            for (int i = 0; i < rustorePayments.purchases.Count; i++)
            {
                GameObject go = Instantiate(purchaseCardPrefab, purchasesCardsContainer);

                go.GetComponent<UIPurchaseCard>().SetData(rustorePayments.purchases[i]);
            }
        }
    }
}
