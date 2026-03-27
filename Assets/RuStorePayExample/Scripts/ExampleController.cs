using RuStore.CoreClient;
using RuStore.PayClient;
using RuStore.PayExample.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuStore.PayExample
{

    public class ExampleController : MonoBehaviour
    {
        [SerializeField]
        private string logTag;

        [SerializeField]
        private string[] productsId;

        [SerializeField]
        private CardsView productsView;

        [SerializeField]
        private CardsView purchasesView;

        [SerializeField]
        private PurchaseMethodBox purchaseMethodBox;

        [SerializeField]
        private MessageBox messageBox;

        [SerializeField]
        private LoadingIndicator loadingIndicator;

        [SerializeField]
        private Text isPurchaseAvailability;

        [SerializeField]
        private Text isRuStoreInstalledLabel;

        [SerializeField]
        private ProductTypeView productTypeView;

        [SerializeField]
        private PurchaseStatusView purchaseStatusView;

        [SerializeField]
        private ProductInfoBox _productInfoBox;

        private void Start()
        {
            ProductCardView.OnBuyProduct += ProductCardView_OnBuyProduct;
            ProductCardView.OnInfoProduct += ProductCardView_OnInfoProduct;

            PurchaseCardView.OnConfirmPurchase += PurchaseCardView_OnConfirmPurchase;
            PurchaseCardView.OnCancelPurchase += PurchaseCardView_OnCancelPurchase;
            PurchaseCardView.OnGetPurchase += PurchaseCardView_OnGetPurchase;

            productTypeView.onValueChangedEvent += ProductTypeView_onValueChangedEvent;
            purchaseStatusView.onValueChangedEvent += PurchaseStatusView_onValueChangedEvent;

            RuStorePayClient.Instance.GetPurchaseAvailability(
                onFailure: (error) =>
                {
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    var message = "Purchases are available: " + result.isAvailable.ToString();
                    isPurchaseAvailability.text = message;
                });

            var isRuStoreInstalled = RuStoreCoreClient.Instance.IsRuStoreInstalled();
            var message = isRuStoreInstalled ? "RuStore is installed [v]" : "RuStore is not installed [x]";
            isRuStoreInstalledLabel.text = message;
        }

        private void PurchaseStatusView_onValueChangedEvent(object sender, Enum e) => LoadPurchases();

        private void ProductTypeView_onValueChangedEvent(object sender, ProductType? e) => LoadPurchases();

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

        public void GetPurchaseAvailability()
        {
            loadingIndicator.Show();

            RuStorePayClient.Instance.GetPurchaseAvailability(
                onFailure: (error) =>
                {
                    loadingIndicator.Hide();
                    OnRuStorePaymentException(error);
                },
                onSuccess: (result) =>
                {
                    loadingIndicator.Hide();

                    if (result.isAvailable)
                    {
                        messageBox.Show("Availability", "True");
                    }
                    else
                    {
                        OnRuStorePaymentException(result.cause);
                    }
                });
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

        private void ProductCardView_OnInfoProduct(object sender, EventArgs e)
        {
            var product = (sender as ICardView<Product>).GetData();

            var json = DataSerializer.SerializeToJson(product, true);
            Logcat.LogWarning(logTag, json);

            _productInfoBox.Show(product);
        }

        private void ProductCardView_OnBuyProduct(object sender, EventArgs e)
        {

            var product = (sender as ICardView<Product>).GetData();

            var parameters = new ProductPurchaseParams(productId: product.productId);

            Action<RuStoreError> onError = (error) =>
            {
                loadingIndicator.Hide();
                OnRuStorePaymentException(error);
            };

            Action<ProductPurchaseResult> onSuccess = (result) =>
            {
                loadingIndicator.Hide();
                var jsonResult = DataSerializer.SerializeToJson(result, true);
                Logcat.LogWarning(logTag, jsonResult);
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

                    var jsonResult = DataSerializer.SerializeToJson(result.Count, true);
                    Logcat.LogWarning(logTag, jsonResult);
                });
        }

        private void PurchaseCardView_OnGetPurchase(object sender, EventArgs e)
        {
            loadingIndicator.Show();

            var purchase = (sender as ICardView<IPurchase>).GetData();

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

                    var jsonResult = DataSerializer.SerializeToJson(result, true);
                    Logcat.LogWarning(logTag, jsonResult);
                });
        }

        private void PurchaseCardView_OnConfirmPurchase(object sender, EventArgs e)
        {
            loadingIndicator.Show();

            var purchase = (sender as ICardView<IPurchase>).GetData();
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

        private void PurchaseCardView_OnCancelPurchase(object sender, EventArgs e)
        {
            loadingIndicator.Show();

            var purchase = (sender as ICardView<IPurchase>).GetData();
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

        private void OnError(RuStoreError error)
        {
            messageBox.Show(error.name, error.description);
            Debug.LogErrorFormat("{0} : {1}", error.name, error.description);
        }

        private void OnRuStorePaymentException(RuStoreError error)
        {
            var message = "";
            switch (error)
            {
                case RuStorePaymentException.RuStorePaymentNetworkException networkException:
                    message = string.Format(
                        "{0}\ncode: {1}\nid: {2}",
                        networkException.description,
                        networkException.code,
                        networkException.id
                    );

                    messageBox.Show(error.name, message);
                    Debug.LogErrorFormat("{0} : {1}", error.name, message);
                    break;

                case RuStorePaymentException.ProductPurchaseException productPurchaseException:
                    message = string.Format(
                        "Sandbox: {0}",
                        productPurchaseException.sandbox?.ToString() ?? "null"
                    );

                    messageBox.Show(error.name, message);
                    Debug.LogErrorFormat("{0} : {1}", error.name, message);
                    break;

                default:
                    OnError(error);
                    break;
            }
        }
    }
}
