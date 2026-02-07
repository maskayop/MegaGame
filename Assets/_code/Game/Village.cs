using UnityEngine;

namespace MegaGame
{
    public class Village : BaseCharacter
    {
        [Header("Money")]
        public short revenue = 1;

        [Header("Visual")]
        [SerializeField] GameObject playerVisual;
        [SerializeField] GameObject enemyVisual;
        [SerializeField] GameObject neutralVisual;

        [Header("Widgets")]
        [SerializeField] HealthIndicatorWidget playerHealthWidget;
        [SerializeField] HealthIndicatorWidget enemyHealthWidget;
        [SerializeField] HealthIndicatorWidget neutralHealthWidget;

        [HideInInspector] public Island island;

        int currentDay = 0;
        bool isCaptured = false;

        protected override void OnAwake() { }

        protected override void OnStart()
        {
            Init();
        }

        protected override void OnInit()
        {
            isCaptured = false;
            SetVisual();
        }

        protected override void OnUpdate()
        {
            if (isCaptured)
                return;

            if (currentHealth <= 0)
            {
                Kill();
                //return;
            }

            UpdateHealthWidget();
            UpdateProperties();
        }

        void Kill()
        {
            UpdateHealthWidget();
            isCaptured = true;
        }

        void UpdateHealthWidget()
        {
            if (GetCurrentHealthWidget() == null)
                return;

            if (isCaptured)
            {
                Debug.Log("!!!");
                GetCurrentHealthWidget().SetValue(0);
                return;
            }

            if (currentHealth != health)
            {
                GetCurrentHealthWidget().SetValue(currentHealth / health);
                GetCurrentHealthWidget().gameObject.SetActive(true);
            }
            else
                GetCurrentHealthWidget().gameObject.SetActive(false);
        }

        void UpdateProperties()
        {
            if (globalTime.currentDay != currentDay)
            {
                currentHealth += healthRegeneration;
                currentHealth = Mathf.Clamp(currentHealth, 0, health);

                currentDay = globalTime.currentDay;
            }
        }

        HealthIndicatorWidget GetCurrentHealthWidget()
        {
            if (owner == Owner.player)
                return playerHealthWidget;
            else if (owner == Owner.enemy)
                return enemyHealthWidget;
            else if (owner == Owner.neutral)
                return neutralHealthWidget;
            else
                return null;
        }

        void SetVisual()
        {
            playerVisual.gameObject.SetActive(false);
            enemyVisual.gameObject.SetActive(false);
            neutralVisual.gameObject.SetActive(false);

            if (owner == Owner.player)
                playerVisual.gameObject.SetActive(true);
            else if (owner == Owner.enemy)
                enemyVisual.gameObject.SetActive(true);
            else if (owner == Owner.neutral)
                neutralVisual.gameObject.SetActive(true);
        }
    }
}
