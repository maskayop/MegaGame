using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class BaseCharacter : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }
        public Owner owner;

        [Header("Health")]
        public float health = 10;
        public float currentHealth = 10;
        public float healthRegeneration = 1;

        [Header("Damage")]
        public float damage = 1.0f;
        public float attackDelay = 1.0f;

        [Header("Info")]
        public List<BaseCharacter> targetEnemies = new List<BaseCharacter>();

        protected GameController gameController;
        protected GlobalTimeController globalTime;

        void Awake()
        {
            OnAwake();
        }

        void Start()
        {
            OnStart();
        }

        void Update()
        {
            for (int i = 0; i < targetEnemies.Count; i++)
                if (!targetEnemies[i])
                    targetEnemies.Remove(targetEnemies[i]);

            OnUpdate();
        }

        public void Init()
        {
            currentHealth = health;

            if (GlobalTimeController.Instance)
                globalTime = GlobalTimeController.Instance;

            if (GameController.Instance)
                gameController = GameController.Instance;

            OnInit();
        }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }

        protected virtual void OnInit() { }

        protected virtual void OnUpdate() { }
    }
}
