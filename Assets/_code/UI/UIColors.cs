using UnityEngine;
using static MegaGame.BaseCharacter;

namespace MegaGame.UI
{
    public class UIColors : MonoBehaviour
    {
        public static UIColors Instance { get; private set; }

        [Header("String Formats")]
        [SerializeField] string defaultColorFormat = "<color=white>";

        [Space(10)]
        [SerializeField] string moneyGrowthColorFormat = "<color=green>";
        [SerializeField] string moneyWasteColorFormat = "<color=red>";

        [Space(10)]
        [SerializeField] string playerColorFormat = "<color=green>";
        [SerializeField] string enemyColorFormat = "<color=red>";
        [SerializeField] string neutralColorFormat = "<color=yellow>";

        [Header("Colors")]
        [SerializeField] Color playerColor = Color.red;
        [SerializeField] Color enemyColor = Color.blue;
        [SerializeField] Color neutralColor = Color.yellow;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIColors");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public string GetMoneyGrowthColorString()
        {
            return moneyGrowthColorFormat;
        }

        public string GetMoneyWasteColorString()
        {
            return moneyWasteColorFormat;
        }

        public Color GetTextOwnerColor(Owner targetOwner)
        {
            if (targetOwner == Owner.player)
                return playerColor;
            else if (targetOwner == Owner.enemy)
                return enemyColor;
            else if (targetOwner == Owner.neutral)
                return neutralColor;
            else
                return Color.white;
        }

        public string GetTextOwnerColorString(Owner targetOwner)
        {
            if (targetOwner == Owner.player)
                return playerColorFormat;
            else if (targetOwner == Owner.enemy)
                return enemyColorFormat;
            else if (targetOwner == Owner.neutral)
                return neutralColorFormat;
            else
                return defaultColorFormat;
        }

        public string GetDefaultColorString()
        {
            return defaultColorFormat;
        }
    }
}
