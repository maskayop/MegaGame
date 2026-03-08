using UnityEngine;
using static MegaGame.BaseCharacter;

namespace MegaGame
{
    [RequireComponent(typeof(BaseSettlement))]
    public class SettlementVisual : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] GameObject playerVisual;
        [SerializeField] GameObject enemyVisual;
        [SerializeField] GameObject neutralVisual;

        [Header("Constructions")]
        [SerializeField] GameObject fortVisual;

        BaseSettlement settlement;
        SettlementConstructions settlementConstructions;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            settlement = GetComponent<BaseSettlement>();
            settlementConstructions = GetComponent<SettlementConstructions>();
        }

        public void SetVisual()
        {
            playerVisual.gameObject.SetActive(false);
            enemyVisual.gameObject.SetActive(false);
            neutralVisual.gameObject.SetActive(false);

            if (settlement.owner == Owner.player)
                playerVisual.gameObject.SetActive(true);
            else if (settlement.owner == Owner.enemy)
                enemyVisual.gameObject.SetActive(true);
            else if (settlement.owner == Owner.neutral)
                neutralVisual.gameObject.SetActive(true);

            SetFortVisual();
        }

        public void SetFortVisual()
        {
            if (!settlementConstructions)
                return;

            if (!fortVisual)
                return;

            if (!settlementConstructions.fortIsBuilt)
                fortVisual.gameObject.SetActive(false);
            else
                fortVisual.gameObject.SetActive(true);
        }
    }
}
