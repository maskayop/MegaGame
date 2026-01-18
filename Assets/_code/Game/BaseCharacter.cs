using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class BaseCharacter : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }
        public Owner owner;

        [SerializeField] HealthIndicatorWidget healthIndicatorWidget;

        [Header("Health")]
        public float health = 10;
        public float currentHealth = 10;
        public float healthRegeneration = 1;

        [Header("Speed")]
        public float speed = 1;
        public float currentSpeed = 1;
        public float speedDrop = 5.0f;

        [Header("Damage")]
        public float damage = 1.0f;
        public float attackDelay = 1.0f;

        [Header("Info")]
        public List<Character> targetEnemies = new List<Character>();

        GlobalTimeController globalTime;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            currentHealth = health;

            if (GlobalTimeController.Instance)
                globalTime = GlobalTimeController.Instance;
        }
    }
}
