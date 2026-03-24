using RuStore;
using RuStore.PayClient;
using System;
using UnityEngine;

public class RustorePayments : MonoBehaviour
{
    public static RustorePayments Instance { get; private set; }

    public bool StoreAvailability;

    public static event Action OnStoreCheckStarted;

    public static event Action OnStoreAvailable;
    public static event Action<string> OnStoreAvailableError;
    public static event Action<RuStoreError> OnStoreConnectionFailed;

    public static event Action OnUserAuthorized;
    public static event Action OnUserUnauthorized;
    public static event Action<RuStoreError> OnUserAuthorizationFailed;

    public static event Action OnProductsLoaded;
    public static event Action<RuStoreError> OnProductsLoadingError;

    public static event Action OnGetUserPurchasesSuccess;
    public static event Action OnGetUserSubscriptionPurchasesSuccess;
    public static event Action<RuStoreError> OnGetUserPurchasesFailed;

    public ProductId[] productIds;

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
        CheckStoreAvailability();
        GetUserAuthorizationStatus();
        LoadProducts();
        GetPurchases();
    }

    void CheckStoreAvailability()
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
                    StoreAvailability = true;
                    OnStoreAvailable?.Invoke();
                }
                else
                {
                    StoreAvailability = false;
                    string errorReason = GetReasonMessage(response.cause);
                    OnStoreAvailableError?.Invoke(errorReason);
                }
            }
        );
    }

    void GetUserAuthorizationStatus()
    {
        RuStorePayClient.Instance.GetUserAuthorizationStatus(
            onFailure: (error) =>
            {
                OnUserAuthorizationFailed?.Invoke(error);
            },
            onSuccess: (result) =>
            {
                if (result == UserAuthorizationStatus.AUTHORIZED)
                    OnUserAuthorized?.Invoke();
                else
                    OnUserUnauthorized?.Invoke();
            });
    }

    void LoadProducts()
    {
        RuStorePayClient.Instance.GetProducts(
            productsId: productIds,
            onFailure: (error) =>
            {
                OnProductsLoadingError?.Invoke(error);
            },
            onSuccess: (result) =>
            {
                OnProductsLoaded?.Invoke();
            });
    }

    void GetPurchases()
    {
        RuStorePayClient.Instance.GetPurchases(
            onFailure: (error) =>
            {
                OnGetUserPurchasesFailed?.Invoke(error);
            },
            onSuccess: (result) =>
            {
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
}
