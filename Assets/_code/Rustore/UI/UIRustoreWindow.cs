using RuStore.PayClient;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIRustoreWindow : MonoBehaviour
    {
        public static UIRustoreWindow Instance { get; private set; }

        [SerializeField] TextMeshProUGUI text;

        [Header("Products Cards")]
        [SerializeField] GameObject productCardPrefab;
        [SerializeField] UICardsView productsView;
        [SerializeField] Transform productCardsContainer;

        [Header("Purchases Cards")]
        [SerializeField] GameObject purchaseCardPrefab;
        [SerializeField] UICardsView purchasesView;
        [SerializeField] Transform purchasesCardsContainer;

        [Header("Panels")]
        [SerializeField] UIShopLoadingIndicator loadingIndicator;
        [SerializeField] UIPurchaseMethodBox purchaseMethodBox;
        [SerializeField] UIMessageBox messageBox;
        [SerializeField] UIProductInfoBox productInfoBox;

        [Header("Purchases Filters")]
        [SerializeField] UIProductTypeView productTypeView;
        [SerializeField] UIPurchaseStatusView purchaseStatusView;

        GameShop gameshop;
        RustorePayments rustorePayments;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIRustoreWindow");
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
            RustorePayments.Instance?.LoadProducts();
        }

        public void ReloadRustoreValidation()
        {
            RustorePayments.Instance?.Init();
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

        public void ShowLoadingIndicator(bool state)
        {
            if (!loadingIndicator)
                return;

            if (state)
                loadingIndicator.Show();
            else
                loadingIndicator.Hide();
        }

        public void ShowPurchaseMethodBox(
            string INtitle,
            Action<SdkTheme> INonPreferredOneStep = null,
            Action<SdkTheme> INonPreferredTwoStep = null,
            Action<SdkTheme> INonTwoStep = null,
            Action onCancel = null
            )
        {
            purchaseMethodBox?.Show(INtitle, INonPreferredOneStep, INonPreferredTwoStep, INonTwoStep, onCancel);
        }

        public void ShowMessageBox(string title = null, string message = null, Action onClose = null)
        {
            messageBox?.Show(title, message, onClose);
        }

        public void ShowProductInfoBox(Product product)
        {
            productInfoBox?.Show(product);
        }

        public void SetProductsViewData<T>(List<T> data)
        {
            productsView?.SetData(data);
        }

        public void SetPurchasesViewData<T>(List<T> data)
        {
            purchasesView?.SetData(data);
        }

        public UIProductTypeView GetUIProductTypeView() => productTypeView;

        public UIPurchaseStatusView GetUIPurchaseStatusView() => purchaseStatusView;
    }
}
