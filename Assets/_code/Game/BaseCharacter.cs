using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class BaseCharacter : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }
        public Owner owner;

        [Header("Money")]
        public short revenue = 1;
        public short maintenance = 1;

        [Header("Health")]
        public float health = 10;
        public float currentHealth = 10;
        public float healthRegeneration = 1;

        [Header("Damage")]
        public float damage = 1.0f;
        public float attackDelay = 1.0f;

        [Header("Widgets")]
        [SerializeField] protected HealthIndicatorWidget playerHealthWidget;
        [SerializeField] protected HealthIndicatorWidget enemyHealthWidget;
        [SerializeField] protected HealthIndicatorWidget neutralHealthWidget;

        [Header("Info")]
        public List<BaseCharacter> targetEnemies = new List<BaseCharacter>();

        protected GameController gameController;
        protected GlobalTimeController globalTime;

        float currentHealthNormalized;

        protected int currentDay = 0;

        Island island;
        public Island Island { get { return island; } set { island = value; } }

        float currentAttackTime = 0;

        void Awake()
        {
            OnAwake();
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (health != 0)
                currentHealthNormalized = currentHealth / health;
            else
                return;

            if (currentHealth <= 0)
                return;

            for (int i = 0; i < targetEnemies.Count; i++)
            {
                if (!targetEnemies[i])
                    targetEnemies.Remove(targetEnemies[i]);
                else if (targetEnemies[i].owner == owner)
                    targetEnemies.Remove(targetEnemies[i]);
            }

            if (targetEnemies.Count != 0)
                currentAttackTime -= Time.deltaTime;

            if (currentAttackTime < 0)
                Attack();

            UpdateHealthWidget();
            UpdateProperties();
            UpdateTargets();

            OnUpdate();
        }

        public void Init()
        {
            currentHealth = health;

            if (GlobalTimeController.Instance)
                globalTime = GlobalTimeController.Instance;

            currentDay = globalTime.currentDay;

            if (GameController.Instance)
                gameController = GameController.Instance;

            OnInit();
        }

        protected virtual void OnAwake() { }

        protected virtual void OnInit() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnTriggerEnter(Collider coll)
        {
            BaseCharacter targetCharacter = coll.GetComponentInParent<BaseCharacter>();

            if (targetCharacter)
            {
                if (owner == Owner.player)
                {
                    if (targetCharacter.owner != Owner.player)
                        targetEnemies.Add(targetCharacter);
                }
                else if (owner == Owner.enemy)
                {
                    if (targetCharacter.owner != Owner.enemy)
                        targetEnemies.Add(targetCharacter);
                }
            }
        }

        protected virtual void OnTriggerExit(Collider coll)
        {
            BaseCharacter targetCharacter = coll.GetComponentInParent<BaseCharacter>();

            if (targetCharacter)
                targetEnemies.Remove(targetCharacter);
        }

        protected void UpdateHealthWidget()
        {
            if (GetCurrentHealthWidget() == null)
                return;

            if (currentHealth != health)
            {
                if (currentHealth <= 0)
                    GetCurrentHealthWidget().SetValue(0);
                else
                    GetCurrentHealthWidget().SetValue(currentHealthNormalized);

                GetCurrentHealthWidget().gameObject.SetActive(true);
            }
            else
                GetCurrentHealthWidget().gameObject.SetActive(false);
        }

        HealthIndicatorWidget GetCurrentHealthWidget()
        {
            if (owner == Owner.player && playerHealthWidget)
                return playerHealthWidget;
            else if (owner == Owner.enemy && enemyHealthWidget)
                return enemyHealthWidget;
            else if (owner == Owner.neutral && neutralHealthWidget)
                return neutralHealthWidget;
            else
                return null;
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

        void UpdateTargets()
        {
            for (int i = 0; i < targetEnemies.Count; i++)
            {
                if (targetEnemies[i] == null)
                    targetEnemies.Remove(targetEnemies[i]);
            }
        }

        void Kill()
        {
            currentHealth = 0;
            UpdateHealthWidget();
            OnKilled();
        }

        protected virtual void OnKilled() { }

        void Attack()
        {
            if (targetEnemies.Count != 0)
                targetEnemies[0].DealDamage(damage);

            currentAttackTime = attackDelay;

            OnAttack();
        }

        protected virtual void OnAttack() { }

        public void DealDamage(float INdamage)
        {
            currentHealth -= INdamage;

            if (currentHealth <= 0)
            {
                UpdateHealthWidget();
                Kill();
            }
        }
    }
}
