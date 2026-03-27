using MegaGame.UI;
using RuStore;
using RuStore.PayClient;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class RustorePayments : MonoBehaviour
    {
        public static RustorePayments Instance { get; private set; }

        [SerializeField] string logTag = "json";
        [SerializeField] UIShopLoadingIndicator loadingIndicator;
        [SerializeField] UIPurchaseMethodBox purchaseMethodBox;
        [SerializeField] UIMessageBox messageBox;
        [SerializeField] UIProductInfoBox productInfoBox;
        [SerializeField] UICardsView purchasesView;
        [SerializeField] UIProductTypeView productTypeView;
        [SerializeField] UIPurchaseStatusView purchaseStatusView;
        [SerializeField] UICardsView productsView;

        public static event Action OnStoreCheckStarted;

        public static event Action OnStoreAvailable;
        public static event Action<string> OnStoreAvailableError;
        public static event Action<RuStoreError> OnStoreConnectionFailed;

        public static event Action OnProductsLoaded;
        public static event Action<RuStoreError> OnProductsLoadingError;

        public static event Action OnGetUserPurchasesSuccess;
        public static event Action OnGetUserSubscriptionPurchasesSuccess;
        public static event Action<RuStoreError> OnGetUserPurchasesFailed;

        bool rustoreIsAvailable;
        public bool RustoreIsAvailable { get { return rustoreIsAvailable; } }

        public string[] productsId;

        public string[] currentRustoreStatus = new string[4];

        public List<Product> products = new List<Product>();
        public List<IPurchase> purchases = new List<IPurchase>();

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create RustorePayments");
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
            RustoreValidation();
            CheckStoreAvailability();
            GetUserAuthorizationStatus();
            LoadProducts();
            GetPurchases();
        }

        public void RustoreValidation()
        {
            //---

            OnStoreAvailable -= OnRustoreAvailable;
            OnStoreAvailableError -= OnRustoreAvailableError;
            OnStoreConnectionFailed -= OnRustoreConnectionFailed;

            OnProductsLoaded -= OnRustoreProductsLoaded;
            OnProductsLoadingError -= OnRustoreProductsLoadingError;

            OnGetUserPurchasesSuccess -= OnRustoreGetUserPurchasesSuccess;
            OnGetUserSubscriptionPurchasesSuccess -= OnRustoreGetUserSubscriptionPurchasesSuccess;
            OnGetUserPurchasesFailed -= OnRustoreGetUserPurchasesFailed;

            UIProductCard.OnBuyProduct -= ProductCard_OnBuyProduct;
            UIProductCard.OnInfoProduct -= ProductCard_OnInfoProduct;

            UIPurchaseCard.OnConfirmPurchase -= PurchaseCard_OnConfirmPurchase;
            UIPurchaseCard.OnCancelPurchase -= PurchaseCard_OnCancelPurchase;
            UIPurchaseCard.OnGetPurchase -= PurchaseCard_OnGetPurchase;

            productTypeView.onValueChangedEvent -= ProductTypeView_onValueChangedEvent;
            purchaseStatusView.onValueChangedEvent -= PurchaseStatusView_onValueChangedEvent;

            //+++

            OnStoreAvailable += OnRustoreAvailable;
            OnStoreAvailableError += OnRustoreAvailableError;
            OnStoreConnectionFailed += OnRustoreConnectionFailed;

            OnProductsLoaded += OnRustoreProductsLoaded;
            OnProductsLoadingError += OnRustoreProductsLoadingError;

            OnGetUserPurchasesSuccess += OnRustoreGetUserPurchasesSuccess;
            OnGetUserSubscriptionPurchasesSuccess += OnRustoreGetUserSubscriptionPurchasesSuccess;
            OnGetUserPurchasesFailed += OnRustoreGetUserPurchasesFailed;

            UIProductCard.OnBuyProduct += ProductCard_OnBuyProduct;
            UIProductCard.OnInfoProduct += ProductCard_OnInfoProduct;

            UIPurchaseCard.OnConfirmPurchase += PurchaseCard_OnConfirmPurchase;
            UIPurchaseCard.OnCancelPurchase += PurchaseCard_OnCancelPurchase;
            UIPurchaseCard.OnGetPurchase += PurchaseCard_OnGetPurchase;

            productTypeView.onValueChangedEvent += ProductTypeView_onValueChangedEvent;
            purchaseStatusView.onValueChangedEvent += PurchaseStatusView_onValueChangedEvent;
        }

        public void CheckStoreAvailability()
        {
            OnStoreCheckStarted?.Invoke();

            RuStorePayClient.Instance.GetPurchaseAvailability(
                onFailure: (error) =>
                {
                    OnStoreConnectionFailed?.Invoke(error);
                },
                onSuccess: (response) =>
                {
                    if (response.isAvailable)
                    {
                        rustoreIsAvailable = true;
                        OnStoreAvailable?.Invoke();
                    }
                    else
                    {
                        rustoreIsAvailable = false;
                        string errorReason = GetReasonMessage(response.cause);
                        OnStoreAvailableError?.Invoke(errorReason);
                    }
                }
            );
        }

        public void GetUserAuthorizationStatus()
        {
            loadingIndicator.Show();

            RuStorePayClient.Instance.GetUserAuthorizationStatus(
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    loadingIndicator.Hide();
                    messageBox.Show("UserAuthorizationStatus", result.ToString());
                });
        }

        public void LoadProducts()
        {
            var ids = Array.ConvertAll(productsId, p => new ProductId(p));

            RuStorePayClient.Instance.GetProducts(
                productsId: ids,
                onFailure: (error) =>
                {
                    OnProductsLoadingError?.Invoke(error);
                },
                onSuccess: (result) =>
                {
                    OnProductsLoaded?.Invoke();
                    products = result;
                });
        }

        public void GetPurchases()
        {
            RuStorePayClient.Instance.GetPurchases(
                onFailure: (error) =>
                {
                    OnGetUserPurchasesFailed?.Invoke(error);
                },
                onSuccess: (result) =>
                {
                    purchases = result;

                    result.ForEach(purchase =>
                    {
                        if (purchase is ProductPurchase productPurchase)
                            OnGetUserPurchasesSuccess();
                        if (purchase is SubscriptionPurchase subscriptionPurchase)
                            OnGetUserSubscriptionPurchasesSuccess();
                    });
                });
        }

        string GetReasonMessage(RuStoreError cause)
        {
            return cause.name;
        }

        public void GetProducts()
        {
            loadingIndicator.Show();

            var ids = Array.ConvertAll(productsId, p => new ProductId(p));

            RuStorePayClient.Instance.GetProducts(
                productsId: ids,
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    loadingIndicator.Hide();
                    productsView.SetData(result);
                });
        }

        void ProductCard_OnInfoProduct(object sender, EventArgs e)
        {
            var product = (sender as IProductCard<Product>).GetData();

            var json = RustoreDataSerializer.SerializeToJson(product, true);
            RustoreLogger.LogWarning(logTag, json);

            productInfoBox.Show(product);
        }

        void ProductCard_OnBuyProduct(object sender, EventArgs e)
        {
            var product = (sender as IProductCard<Product>).GetData();
            var parameters = new ProductPurchaseParams(productId: product.productId);

            Action<RuStoreError> onError = (error) =>
            {
                loadingIndicator.Hide();
                OnRuStorePaymentException(error);
            };

            Action<ProductPurchaseResult> onSuccess = (result) =>
            {
                loadingIndicator.Hide();
                var jsonResult = RustoreDataSerializer.SerializeToJson(result, true);
                RustoreLogger.LogWarning(logTag, jsonResult);
            };

            Action<SdkTheme> onPreferredOneStep = (sdkTheme) =>
            {
                loadingIndicator.Show();
                RuStorePayClient.Instance.Purchase(parameters, PreferredPurchaseType.ONE_STEP, sdkTheme, onError, onSuccess);
            };

            Action<SdkTheme> onPreferredOTwoStep = (sdkTheme) =>
            {
                loadingIndicator.Show();
                RuStorePayClient.Instance.Purchase(parameters, PreferredPurchaseType.TWO_STEP, sdkTheme, onError, onSuccess);
            };

            Action<SdkTheme> onTwoStep = (sdkTheme) =>
            {
                loadingIndicator.Show();
                RuStorePayClient.Instance.PurchaseTwoStep(parameters, sdkTheme, onError, onSuccess);
            };

            purchaseMethodBox?.Show(product.title.value, onPreferredOneStep, onPreferredOTwoStep, onTwoStep);
        }

        void OnError(RuStoreError error)
        {
            messageBox.Show(error.name, error.description);
            Debug.LogErrorFormat("{0} : {1}", error.name, error.description);
        }

        void OnRuStorePaymentException(RuStoreError error)
        {
            var message = "";
            switch (error)
            {
                case RuStorePaymentException.RuStorePaymentNetworkException networkException:
                    message = string.Format("{0}\ncode: {1}\nid: {2}", networkException.description, networkException.code, networkException.id);

                    messageBox.Show(error.name, message);
                    Debug.LogErrorFormat("{0} : {1}", error.name, message);
                    break;

                case RuStorePaymentException.ProductPurchaseException productPurchaseException:
                    message = string.Format("Sandbox: {0}", productPurchaseException.sandbox?.ToString() ?? "null");

                    messageBox.Show(error.name, message);
                    Debug.LogErrorFormat("{0} : {1}", error.name, message);
                    break;

                default:
                    OnError(error);
                    break;
            }
        }

        public void LoadPurchases()
        {
            loadingIndicator.Show();
            RuStorePayClient.Instance.GetPurchases(
                productType: productTypeView.GetState(),
                purchaseStatus: purchaseStatusView.GetState(),
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    loadingIndicator.Hide();
                    purchasesView.SetData(result);

                    var jsonResult = RustoreDataSerializer.SerializeToJson(result.Count, true);
                    RustoreLogger.LogWarning(logTag, jsonResult);
                });
        }

        void PurchaseCard_OnGetPurchase(object sender, EventArgs e)
        {
            loadingIndicator.Show();

            var purchase = (sender as IProductCard<IPurchase>).GetData();

            RuStorePayClient.Instance.GetPurchase(
                purchaseId: purchase.purchaseId,
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    loadingIndicator.Hide();
                    messageBox.Show("Purchase", string.Format("Purchase id: {0}", result.purchaseId));

                    var jsonResult = RustoreDataSerializer.SerializeToJson(result, true);
                    RustoreLogger.LogWarning(logTag, jsonResult);
                });
        }

        void PurchaseCard_OnConfirmPurchase(object sender, EventArgs e)
        {
            loadingIndicator.Show();

            var purchase = (sender as IProductCard<IPurchase>).GetData();
            RuStorePayClient.Instance.ConfirmTwoStepPurchase(
                purchaseId: purchase.purchaseId,
                developerPayload: null,
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: () =>
                {
                    loadingIndicator.Hide();
                    LoadPurchases();
                });
        }

        void PurchaseCard_OnCancelPurchase(object sender, EventArgs e)
        {
            loadingIndicator.Show();

            var purchase = (sender as IProductCard<IPurchase>).GetData();
            RuStorePayClient.Instance.CancelTwoStepPurchase(
                purchaseId: purchase.purchaseId,
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: () =>
                {
                    loadingIndicator.Hide();
                    LoadPurchases();
                });
        }

        void PurchaseStatusView_onValueChangedEvent(object sender, Enum e) => LoadPurchases();

        void ProductTypeView_onValueChangedEvent(object sender, ProductType? e) => LoadPurchases();

        void OnRustoreAvailable()
        {
            currentRustoreStatus[0] = "<color=#009922>" + "Rustore is Ready" + "</color>";
        }

        void OnRustoreAvailableError(string reason)
        {
            currentRustoreStatus[0] = "<color=#990022>" + "Rustore is Not ready" + "</color>" + " - " + reason;
        }

        void OnRustoreConnectionFailed(RuStoreError error)
        {
            currentRustoreStatus[0] = "<color=#990022>" + "Rustore is Not ready" + "</color>" + " - " + error.name + " - " + error.description;
        }

        void OnRustoreUserAuthorized()
        {
            currentRustoreStatus[1] = "<color=#009922>" + "User is Authorized" + "</color>";
        }

        void OnRustoreUserUnauthorized()
        {
            currentRustoreStatus[1] = "<color=#990022>" + "User is Not authorized" + "</color>";
        }

        void OnRustoreUserAuthorizationFailed(RuStoreError error)
        {
            currentRustoreStatus[1] = "<color=#990022>" + "User is Not authorized" + "</color>" + " - " + error.name + " - " + error.description;
        }

        void OnRustoreProductsLoaded()
        {
            currentRustoreStatus[2] = "<color=#009922>" + "Products Successfully loaded" + "</color>";
        }

        void OnRustoreProductsLoadingError(RuStoreError error)
        {
            currentRustoreStatus[2] = "<color=#990022>" + "Products Not loaded" + "</color>" + " - " + error.name + " - " + error.description;
        }

        void OnRustoreGetUserPurchasesSuccess()
        {
            currentRustoreStatus[3] = "<color=#009922>" + "User Purchases Successfully loaded" + "</color>";
        }

        void OnRustoreGetUserSubscriptionPurchasesSuccess()
        {
            currentRustoreStatus[3] = "<color=#009922>" + "User Subscription Purchases Successfully loaded" + "</color>";
        }

        void OnRustoreGetUserPurchasesFailed(RuStoreError error)
        {
            currentRustoreStatus[3] = "<color=#990022>" + "User Purchases Not loaded" + "</color>" + " - " + error.name + " - " + error.description;
        }
    }
}
