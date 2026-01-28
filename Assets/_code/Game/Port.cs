using UnityEngine;

namespace MegaGame
{
    public class Port : BaseCharacter
    {
        [Header("Money")]
        public short revenue = 1;

        [Header("Visual")]
        [SerializeField] GameObject playerVisual;
        [SerializeField] GameObject enemyVisual;
        [SerializeField] GameObject neutralVisual;

        [SerializeField] GameObject playerTarget;
        [SerializeField] GameObject enemyTarget;

        [Header("Widgets")]
        [SerializeField] HealthIndicatorWidget playerHealthWidget;
        [SerializeField] HealthIndicatorWidget enemyHealthWidget;

        [Header("FX")]
        [SerializeField] ParticleSystem FXShot;

        [HideInInspector] public Island island;

        int currentDay = 0;
        bool isCaptured = false;

        float currentAttackTime = 0;

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

            if (currentHealth < 0)
            {
                Kill();
                return;
            }

            UpdateHealthWidget();
            UpdateProperties();

            if (targetEnemies.Count != 0)
                currentAttackTime -= Time.deltaTime;

            if (currentAttackTime < 0)
                Attack();
        }

        void OnTriggerEnter(Collider coll)
        {
            Character targetCharacter = coll.GetComponentInParent<Character>();

            if (targetCharacter)
            {
                if (owner == Owner.player)
                {
                    if (targetCharacter.owner == Owner.enemy)
                        targetEnemies.Add(targetCharacter);
                }
                else if (owner == Owner.enemy)
                {
                    if (targetCharacter.owner == Owner.player)
                        targetEnemies.Add(targetCharacter);
                }
            }
        }

        void OnTriggerExit(Collider coll)
        {
            Character targetCharacter = coll.GetComponentInParent<Character>();

            if (targetCharacter)
                targetEnemies.Remove(targetCharacter);
        }

        void Attack()
        {
            if (targetEnemies.Count != 0)
                targetEnemies[0].currentHealth -= damage;

            currentAttackTime = attackDelay;

            if (FXShot)
                FXShot.Play();
        }

        public void OnClickAction()
        {
            if (owner == Owner.enemy)
                gameController.CreatePlayerShip();
        }

        void Kill()
        {
            isCaptured = true;
        }

        void UpdateHealthWidget()
        {
            if (GetCurrentHealthWidget() == null)
                return;

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

        public void SetVisualAsTarget(bool isTarget, Owner targetOwner)
        {
            playerTarget.SetActive(false);
            enemyTarget.SetActive(false);

            if (!isTarget)
                return;

            if (targetOwner == Owner.player)
                playerTarget.SetActive(true);
            else if (targetOwner == Owner.enemy)
                enemyTarget.SetActive(true);
        }
    }
}
