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

            for (int i = 0; i < items.Count; i++)
                items[i].isPurchased = false;
        }

        public void TryPurchaseItem(Data_Item INitem)
        {
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

        public bool CheckForPurchasing(Data_Item INitem)
        {
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
    }
}
