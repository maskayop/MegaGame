using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class BaseCharacter : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }
        public Owner owner;

        [Header("Money")]
        public int revenue = 1;
        public int maintenance = 1;

        [Header("Health")]
        public float health = 10;
        public float currentHealth = 10;
        public float healthRegeneration = 1;

        protected float maxHealth;
        public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

        protected float maxRegeneration;
        public float MaxRegeneration { get { return maxRegeneration; } set { maxRegeneration = value; } }

        [Header("Damage")]
        public float damage = 1.0f;
        public float neutralDamageDivider = 2.0f;
        public float attackDelay = 1.0f;

        protected float maxDamage;
        public float MaxDamage { get { return maxDamage; } set { maxDamage = value; } }

        [Header("Widgets")]
        [SerializeField] protected HealthIndicatorWidget playerHealthWidget;
        [SerializeField] protected HealthIndicatorWidget enemyHealthWidget;
        [SerializeField] protected HealthIndicatorWidget neutralHealthWidget;

        [Header("Info")]
        public float timeForTargetsUpdate = 3;
        public List<BaseCharacter> targetEnemies = new List<BaseCharacter>();

        protected GameController gameController;
        protected GlobalTimeController globalTime;

        float currentHealthNormalized;
        public float CurrentHealthNormalized { get { return currentHealthNormalized; } }

        protected int currentDay = 0;

        Island island;
        public Island Island { get { return island; } set { island = value; } }

        protected float currentAttackTime = 0;
        protected float currentTargetsUpdateTime = 0;

        protected Collider coll;

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
            if (maxHealth != 0)
                currentHealthNormalized = currentHealth / maxHealth;
            else if (currentHealth <= 0)
                return;

            currentTargetsUpdateTime -= Time.deltaTime;

            if (currentTargetsUpdateTime < 0)
            {
                coll.enabled = false;
                targetEnemies.Clear();
                coll.enabled = true;

                currentTargetsUpdateTime = timeForTargetsUpdate;
            }

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
            maxHealth = health;
            currentHealth = maxHealth;
            maxRegeneration = healthRegeneration;

            if (owner == Owner.neutral)
                maxDamage = damage / neutralDamageDivider;
            else
                maxDamage = damage;

            if (GlobalTimeController.Instance)
                globalTime = GlobalTimeController.Instance;

            currentDay = globalTime.currentDay;

            if (GameController.Instance)
                gameController = GameController.Instance;

            coll = GetComponent<Collider>();

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
                    if (targetCharacter.owner != Owner.player && CanAddTargetToList(targetCharacter))
                        targetEnemies.Add(targetCharacter);
                }
                else if (owner == Owner.enemy)
                {
                    if (targetCharacter.owner != Owner.enemy && CanAddTargetToList(targetCharacter))
                        targetEnemies.Add(targetCharacter);
                }
                else if (owner == Owner.neutral)
                {
                    if (targetCharacter.owner != Owner.neutral && CanAddTargetToList(targetCharacter))
                        targetEnemies.Add(targetCharacter);
                }
            }

            OnTriggerEnterUpdate();
        }

        protected virtual void OnTriggerEnterUpdate() { }

        protected virtual void OnTriggerExit(Collider coll)
        {
            BaseCharacter targetCharacter = coll.GetComponentInParent<BaseCharacter>();

            if (targetCharacter)
                targetEnemies.Remove(targetCharacter);

            OnTriggerExitUpdate();
        }

        protected virtual void OnTriggerExitUpdate() { }

        protected void UpdateHealthWidget()
        {
            if (GetCurrentHealthWidget() == null)
                return;

            if (currentHealth != maxHealth)
            {
                if (currentHealth <= 0)
                {
                    GetCurrentHealthWidget().SetValue(0);
                    GetCurrentHealthWidget().gameObject.SetActive(false);
                }
                else
                    GetCurrentHealthWidget().SetValue(currentHealthNormalized);

                GetCurrentHealthWidget().gameObject.SetActive(true);
            }
            else
                GetCurrentHealthWidget().gameObject.SetActive(false);
        }

        protected HealthIndicatorWidget GetCurrentHealthWidget()
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

        protected virtual void UpdateProperties()
        {
            if (globalTime.currentDay != currentDay)
            {
                currentHealth += maxRegeneration;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

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

        public void Kill()
        {
            if (GetComponent<Transform>() == null)
                return;

            if (gameObject == null)
                return;

            currentHealth = 0;
            UpdateHealthWidget();
            OnKilled();
        }

        protected virtual void OnKilled() { }

        protected virtual void Attack()
        {
            if (targetEnemies.Count != 0)
                if (targetEnemies[0])
                    targetEnemies[0].TakeDamage(maxDamage, this);

            currentAttackTime = attackDelay;

            OnAttack();
        }

        protected virtual void OnAttack() { }

        void TakeDamage(float INdamage, BaseCharacter damager)
        {
            if (!damager)
                return;

            currentHealth -= INdamage;

            if (currentHealth <= 0)
                Kill();

            OnTakeDamage(damager);
        }

        protected virtual void OnTakeDamage(BaseCharacter damager) { }

        protected virtual bool CanAddTargetToList(BaseCharacter targetCharacter) { return true; }
    }
}
