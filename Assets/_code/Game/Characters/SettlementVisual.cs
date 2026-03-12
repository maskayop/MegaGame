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
            if (settlement as PirateLair)
                return;

            if (playerVisual)
                playerVisual.gameObject.SetActive(false);

            if (enemyVisual)
                enemyVisual.gameObject.SetActive(false);

            if (neutralVisual)
                neutralVisual.gameObject.SetActive(false);

            if (settlement.owner == Owner.player)
            {
                if (playerVisual)
                    playerVisual.gameObject.SetActive(true);
            }
            else if (settlement.owner == Owner.enemy)
            {
                if (enemyVisual)
                    enemyVisual.gameObject.SetActive(true);
            }
            else if (settlement.owner == Owner.neutral)
            {
                if (neutralVisual)
                    neutralVisual.gameObject.SetActive(true);
            }

            SetFortVisual();
        }

        public GameObject GetVisualObject()
        {
            if (settlement)
            {
                if (settlement.owner == Owner.player)
                {
                    if (playerVisual)
                        return playerVisual.gameObject;
                }
                else if (settlement.owner == Owner.enemy)
                {
                    if (enemyVisual)
                        return enemyVisual.gameObject;
                }
                else if (settlement.owner == Owner.neutral)
                {
                    if (neutralVisual)
                        return neutralVisual.gameObject;
                }
            }
            else if (neutralVisual)
                return neutralVisual.gameObject;

            return null;
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
