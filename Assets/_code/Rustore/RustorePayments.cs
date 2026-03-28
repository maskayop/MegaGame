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

        public string logTag = "json";

        public static event Action OnStoreCheckStarted;

        public bool rustoreIsAvailable = false;
        public bool RustoreIsAvailable { get { return rustoreIsAvailable; } }

        public bool userIsAuthorized = false;
        public bool UserIsAuthorized { get { return userIsAuthorized; } }

        public string[] productsId;
        public string[] currentRustoreStatus = new string[4];

        public List<Product> products = new List<Product>();
        public List<IPurchase> purchases = new List<IPurchase>();

        UIRustoreWindow rustoreWindowUI;

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
            rustoreWindowUI = UIRustoreWindow.Instance;

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

            UIProductCard.OnBuyProduct -= ProductCard_OnBuyProduct;
            UIProductCard.OnInfoProduct -= ProductCard_OnInfoProduct;

            UIPurchaseCard.OnConfirmPurchase -= PurchaseCard_OnConfirmPurchase;
            UIPurchaseCard.OnCancelPurchase -= PurchaseCard_OnCancelPurchase;
            UIPurchaseCard.OnGetPurchase -= PurchaseCard_OnGetPurchase;

            //+++

            UIProductCard.OnBuyProduct += ProductCard_OnBuyProduct;
            UIProductCard.OnInfoProduct += ProductCard_OnInfoProduct;

            UIPurchaseCard.OnConfirmPurchase += PurchaseCard_OnConfirmPurchase;
            UIPurchaseCard.OnCancelPurchase += PurchaseCard_OnCancelPurchase;
            UIPurchaseCard.OnGetPurchase += PurchaseCard_OnGetPurchase;
        }

        public void CheckStoreAvailability()
        {
            OnStoreCheckStarted?.Invoke();
            rustoreWindowUI?.ShowLoadingIndicator(true);

            RuStorePayClient.Instance.GetPurchaseAvailability(
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRustoreConnectionFailed(error);
                },
                onSuccess: (response) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    if (response.isAvailable)
                    {
                        OnRustoreAvailable();
                    }
                    else
                    {
                        string errorReason = GetReasonMessage(response.cause);
                        OnRustoreAvailableError(errorReason);
                    }
                }
            );
        }

        public void GetUserAuthorizationStatus()
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            RuStorePayClient.Instance.GetUserAuthorizationStatus(
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRuStorePaymentException(error);
                    OnRustoreUserAuthorizationFailed(error);
                },
                onSuccess: (result) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    rustoreWindowUI?.ShowMessageBox("UserAuthorizationStatus", result.ToString());

                    if (result == UserAuthorizationStatus.AUTHORIZED)
                        OnRustoreUserAuthorized();
                    else if (result == UserAuthorizationStatus.UNAUTHORIZED)
                        OnRustoreUserUnauthorized();
                });
        }

        public void LoadProducts()
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            var ids = Array.ConvertAll(productsId, p => new ProductId(p));

            RuStorePayClient.Instance.GetProducts(
                productsId: ids,
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRustoreProductsLoadingError(error);
                },
                onSuccess: (result) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRustoreProductsLoaded();
                    products = result;
                });
        }

        public void GetPurchases()
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            RuStorePayClient.Instance.GetPurchases(
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRustoreGetUserPurchasesFailed(error);
                },
                onSuccess: (result) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    purchases = result;

                    result.ForEach(purchase =>
                    {
                        if (purchase is ProductPurchase productPurchase)
                            OnRustoreGetUserPurchasesSuccess();
                        if (purchase is SubscriptionPurchase subscriptionPurchase)
                            OnRustoreGetUserSubscriptionPurchasesSuccess();
                    });
                });
        }

        string GetReasonMessage(RuStoreError cause)
        {
            return cause.name;
        }

        public void GetProducts()
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            var ids = Array.ConvertAll(productsId, p => new ProductId(p));

            RuStorePayClient.Instance.GetProducts(
                productsId: ids,
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    rustoreWindowUI?.SetProductsViewData(result);
                });
        }

        void ProductCard_OnInfoProduct(object sender, EventArgs e)
        {
            var product = (sender as IProductCard<Product>).GetData();

            var json = RustoreDataSerializer.SerializeToJson(product, true);
            RustoreLogger.LogWarning(logTag, json);

            rustoreWindowUI?.ShowProductInfoBox(product);
        }

        void ProductCard_OnBuyProduct(object sender, EventArgs e)
        {
            var product = (sender as IProductCard<Product>).GetData();
            var parameters = new ProductPurchaseParams(productId: product.productId);

            Action<RuStoreError> onError = (error) =>
            {
                rustoreWindowUI?.ShowLoadingIndicator(false);
                OnRuStorePaymentException(error);
            };

            Action<ProductPurchaseResult> onSuccess = (result) =>
            {
                rustoreWindowUI?.ShowLoadingIndicator(false);

                var jsonResult = RustoreDataSerializer.SerializeToJson(result, true);
                RustoreLogger.LogWarning(logTag, jsonResult);
            };

            Action<SdkTheme> onPreferredOneStep = (sdkTheme) =>
            {
                rustoreWindowUI?.ShowLoadingIndicator(true);
                RuStorePayClient.Instance.Purchase(parameters, PreferredPurchaseType.ONE_STEP, sdkTheme, onError, onSuccess);
            };

            Action<SdkTheme> onPreferredOTwoStep = (sdkTheme) =>
            {
                rustoreWindowUI?.ShowLoadingIndicator(true);
                RuStorePayClient.Instance.Purchase(parameters, PreferredPurchaseType.TWO_STEP, sdkTheme, onError, onSuccess);
            };

            Action<SdkTheme> onTwoStep = (sdkTheme) =>
            {
                rustoreWindowUI?.ShowLoadingIndicator(true);
                RuStorePayClient.Instance.PurchaseTwoStep(parameters, sdkTheme, onError, onSuccess);
            };

            rustoreWindowUI?.ShowPurchaseMethodBox(product.title.value, onPreferredOneStep, onPreferredOTwoStep, onTwoStep);
        }

        void OnError(RuStoreError error)
        {
            rustoreWindowUI?.ShowMessageBox(error.name, error.description);
            Debug.LogErrorFormat("{0} : {1}", error.name, error.description);
        }

        public void OnRuStorePaymentException(RuStoreError error)
        {
            var message = "";
            switch (error)
            {
                case RuStorePaymentException.RuStorePaymentNetworkException networkException:
                    message = string.Format("{0}\ncode: {1}\nid: {2}", networkException.description, networkException.code, networkException.id);

                    rustoreWindowUI?.ShowMessageBox(error.name, message);
                    Debug.LogErrorFormat("{0} : {1}", error.name, message);
                    break;

                case RuStorePaymentException.ProductPurchaseException productPurchaseException:
                    message = string.Format("Sandbox: {0}", productPurchaseException.sandbox?.ToString() ?? "null");

                    rustoreWindowUI?.ShowMessageBox(error.name, message);
                    Debug.LogErrorFormat("{0} : {1}", error.name, message);
                    break;

                default:
                    //OnError(error);
                    break;
            }
        }

        public void LoadPurchases()
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            RuStorePayClient.Instance.GetPurchases(
                productType: rustoreWindowUI?.GetUIProductTypeView().GetState(),
                purchaseStatus: rustoreWindowUI?.GetUIPurchaseStatusView().GetState(),
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    rustoreWindowUI?.SetPurchasesViewData(result);

                    var jsonResult = RustoreDataSerializer.SerializeToJson(result.Count, true);
                    RustoreLogger.LogWarning(logTag, jsonResult);
                });
        }

        void PurchaseCard_OnGetPurchase(object sender, EventArgs e)
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            var purchase = (sender as IProductCard<IPurchase>).GetData();

            RuStorePayClient.Instance.GetPurchase(
                purchaseId: purchase.purchaseId,
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    rustoreWindowUI?.ShowMessageBox("Purchase", string.Format("Purchase id: {0}", result.purchaseId));

                    var jsonResult = RustoreDataSerializer.SerializeToJson(result, true);
                    RustoreLogger.LogWarning(logTag, jsonResult);
                });
        }

        void PurchaseCard_OnConfirmPurchase(object sender, EventArgs e)
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            var purchase = (sender as IProductCard<IPurchase>).GetData();
            RuStorePayClient.Instance.ConfirmTwoStepPurchase(
                purchaseId: purchase.purchaseId,
                developerPayload: null,
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRuStorePaymentException(error);
                },
                onSuccess: () =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    LoadPurchases();
                });
        }

        void PurchaseCard_OnCancelPurchase(object sender, EventArgs e)
        {
            rustoreWindowUI?.ShowLoadingIndicator(true);

            var purchase = (sender as IProductCard<IPurchase>).GetData();
            RuStorePayClient.Instance.CancelTwoStepPurchase(
                purchaseId: purchase.purchaseId,
                onFailure: (error) =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    OnRuStorePaymentException(error);
                },
                onSuccess: () =>
                {
                    rustoreWindowUI?.ShowLoadingIndicator(false);
                    LoadPurchases();
                });
        }

        void OnRustoreAvailable()
        {
            rustoreIsAvailable = true;
            currentRustoreStatus[0] = "<color=#22DD44>" + "Rustore is Ready" + "</color>";
        }

        void OnRustoreAvailableError(string reason)
        {
            rustoreIsAvailable = false;
            currentRustoreStatus[0] = "<color=#DD2244>" + "Rustore is Not ready" + "</color>" + " - " + reason;
        }

        void OnRustoreConnectionFailed(RuStoreError error)
        {
            rustoreIsAvailable = false;
            currentRustoreStatus[0] = "<color=#DD2244>" + "Rustore is Not ready" + "</color>" + " - " + error.name + " - " + error.description;
        }

        void OnRustoreUserAuthorized()
        {
            userIsAuthorized = true;
            currentRustoreStatus[1] = "<color=#22DD44>" + "User is Authorized" + "</color>";
        }

        void OnRustoreUserUnauthorized()
        {
            userIsAuthorized = false;
            currentRustoreStatus[1] = "<color=#DD2244>" + "User is Not authorized" + "</color>";
        }

        void OnRustoreUserAuthorizationFailed(RuStoreError error)
        {
            userIsAuthorized = false;
            currentRustoreStatus[1] = "<color=#DD2244>" + "User is Not authorized" + "</color>" + " - " + error.name + " - " + error.description;
        }

        void OnRustoreProductsLoaded()
        {
            currentRustoreStatus[2] = "<color=#22DD44>" + "Products Successfully loaded" + "</color>";
        }

        void OnRustoreProductsLoadingError(RuStoreError error)
        {
            currentRustoreStatus[2] = "<color=#DD2244>" + "Products Not loaded" + "</color>" + " - " + error.name + " - " + error.description;
        }

        void OnRustoreGetUserPurchasesSuccess()
        {
            currentRustoreStatus[3] = "<color=#22DD44>" + "User Purchases Successfully loaded" + "</color>";
        }

        void OnRustoreGetUserSubscriptionPurchasesSuccess()
        {
            currentRustoreStatus[3] = "<color=#22DD44>" + "User Subscription Purchases Successfully loaded" + "</color>";
        }

        void OnRustoreGetUserPurchasesFailed(RuStoreError error)
        {
            currentRustoreStatus[3] = "<color=#DD2244>" + "User Purchases Not loaded" + "</color>" + " - " + error.name + " - " + error.description;
        }

        public Product GetProductById(string productId)
        {
            for (int i = 0; i < products.Count; i++)
                if (products[i].productId.value == productId)
                    return products[i];

            return null;
        }

        public bool CheckForPurchaseById(string productId)
        {
            for (int i = 0; i < purchases.Count; i++)
            {
                if (GetPurchaseProductId(purchases[i]) == null)
                    return false;

                if (GetPurchaseProductId(purchases[i]) == productId)
                    return true;
            }

            return false;
        }

        string GetPurchaseProductId(IPurchase INpurchase)
        {
            if (INpurchase is ProductPurchase productPurchase) return productPurchase.productId.value;
            if (INpurchase is SubscriptionPurchase subscriptionPurchase) return subscriptionPurchase.productId.value;

            return null;
        }
    }
}
