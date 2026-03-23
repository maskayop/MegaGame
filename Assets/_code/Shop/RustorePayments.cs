using RuStore;
using RuStore.PayClient;
using UnityEngine;

public class RustorePayments : MonoBehaviour
{
    public bool PurchaseAvailability;
    public RuStoreError PurchaseAvailabilityError;

    public bool UserStatusAuthorization;

    public ProductId[] productIds;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        GetPurchaseAvailability();
        GetUserAuthorizationStatus();
        LoadProducts();
        GetPurchases();
    }

    void GetPurchaseAvailability()
    {
        RuStorePayClient.Instance.GetPurchaseAvailability(
            onFailure: (error) =>
            {
                OnPurchaseConnectionError();
            },
            onSuccess: (response) =>
            {
                if (response.isAvailable)
                {
                    PurchaseAvailability = true;
                    OnPurchaseAvailable();
                }
                else
                {
                    PurchaseAvailability = false;
                    PurchaseAvailabilityError = response.cause;
                    OnPurchaseAvailableError();
                }
            }
        );
    }

    void GetUserAuthorizationStatus()
    {
        RuStorePayClient.Instance.GetUserAuthorizationStatus(
            onFailure: (error) =>
            {
                OnUserAuthorizationError();
            },
            onSuccess: (result) =>
            {
                if (result == UserAuthorizationStatus.AUTHORIZED)
                {
                    UserStatusAuthorization = true;
                    OnUserAuthorized();
                }
                else
                {
                    UserStatusAuthorization = false;
                    OnUserUnauthorized();
                }
            });
    }

    void LoadProducts()
    {
        RuStorePayClient.Instance.GetProducts(
            productsId: productIds,
            onFailure: (error) =>
            {
                OnProductsLoadingError();
            },
            onSuccess: (result) =>
            {
                OnProductsLoaded();
            });
    }

    void GetPurchases()
    {
        RuStorePayClient.Instance.GetPurchases(
            onFailure: (error) =>
            {
                OnGetUserPurchasesFailed();
            },
            onSuccess: (result) =>
            {
                result.ForEach(purchase =>
                {
                    if (purchase is ProductPurchase productPurchase)
                        OnGetUserPurchasesSuccess();
                    if (purchase is SubscriptionPurchase subscriptionPurchase)
                        OnGetUserPurchasesError();
                });
            });
    }

    void OnPurchaseConnectionError()
    {

    }

    void OnPurchaseAvailable()
    {

    }

    void OnPurchaseAvailableError()
    {

    }

    void OnUserAuthorizationError()
    {

    }

    void OnUserAuthorized()
    {

    }

    void OnUserUnauthorized()
    {

    }

    void OnProductsLoaded()
    {

    }

    void OnProductsLoadingError()
    {

    }

    void OnGetUserPurchasesFailed()
    {

    }

    void OnGetUserPurchasesSuccess()
    {

    }

    void OnGetUserPurchasesError()
    {

    }
}
