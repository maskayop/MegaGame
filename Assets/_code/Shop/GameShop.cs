using MegaGame.UI;
using RuStore;
using RuStore.PayClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;
namespace MegaGame
{
    [Serializable]
    public class GameplayObjectItem
    {
        public string name;
        public Data_Item item;
        public bool isPurchased;
    }

    public class GameShop : MonoBehaviour
    {
        public static GameShop Instance { get; private set; }

        [Header("Items")]
        [SerializeField] List<GameplayObjectItem> items = new List<GameplayObjectItem>();

        ResourcesController resourcesController;
        GameDataSaver gameDataSaver;
        RustorePayments rustorePayments;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameShop");
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
            resourcesController = ResourcesController.Instance;
            gameDataSaver = GameDataSaver.Instance;
            rustorePayments = RustorePayments.Instance;

            LoadData();
        }

        public void LoadData()
        {
            if (!gameDataSaver)
                return;

            gameDataSaver.LoadLastAccount();
            gameDataSaver.LoadPlayerMoneyData();
            gameDataSaver.LoadShopData();
            gameDataSaver.LoadPremiumShopData();
        }

        public void TryPurchaseItem(Data_Item INitem)
        {
            TryPurchaseGameItem(INitem);
            TryPurchasePremiumItem(INitem);
        }

        void TryPurchaseGameItem(Data_Item INitem)
        {
            if (INitem.IsPremium())
                return;

            if (Strint.Subtraction(resourcesController.PlayerMoney, Strint.GetString(INitem.openGamePrice)) < 0)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                {
                    items[i].isPurchased = true;
                    resourcesController.RemoveMoneyFromPlayer(INitem.openGamePrice);
                    break;
                }
            }
        }

        void TryPurchasePremiumItem(Data_Item INitem)
        {
            if (!INitem.IsPremium())
                return;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                {
#if UNITY_EDITOR
                    items[i].isPurchased = true;
#else
                    TryPurchaseOnRustore(items[i].item);
#endif
                }
            }

            gameDataSaver.SavePremiumShopData();
        }

        void TryPurchaseOnRustore(Data_Item item)
        {
            Product product = rustorePayments.GetProductById(item.rustoreId);
            var card = UIGameShop.Instance.GetUIProductCard(product);

            if (card != null)
            {
                var parameters = new ProductPurchaseParams(productId: product.productId);

                Action<RuStoreError> onError = (error) =>
                {
                    rustorePayments.OnRuStorePaymentException(error);
                };

                Action<ProductPurchaseResult> onSuccess = (result) =>
                {
                    var jsonResult = RustoreDataSerializer.SerializeToJson(result, true);
                    RustoreLogger.LogWarning(rustorePayments.logTag, jsonResult);
                };

                RuStorePayClient.Instance.Purchase(parameters, PreferredPurchaseType.ONE_STEP, onError, onSuccess);
            }
        }

        public void SetPurchasedState(Data_Item INitem, bool state)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                    items[i].isPurchased = state;
            }
        }

        public bool CheckForPurchasing(Data_Item INitem)
        {
            if (INitem.openGamePrice == 0 && INitem.openRealPrice == 0)
                return true;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                {
                    if (items[i].isPurchased)
                        return true;
                }
            }

            return false;
        }

        public bool CheckForAllPremiumItemPurchased()
        {
            UpdateRustorePurchases();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.IsPremium() && !items[i].isPurchased)
                    return false;
            }

            return true;
        }

        bool CanGetRustoreData()
        {
            if (rustorePayments)
            {
                if (rustorePayments.RustoreIsAvailable && rustorePayments.UserIsAuthorized)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void UpdateRustorePurchases()
        {
            if (CanGetRustoreData())
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].item.IsPremium())
                    {
                        items[i].isPurchased = false;

                        if (rustorePayments.CheckForPurchaseById(items[i].item.rustoreId))
                            items[i].isPurchased = true;
                    }
                }
            }
            else
                gameDataSaver?.LoadPremiumShopData();
        }
    }
}
