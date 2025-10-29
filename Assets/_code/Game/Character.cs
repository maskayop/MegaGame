using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MegaGame
{
    public class Character : MonoBehaviour
    {
        public enum Owner { player, enemy }

        public Owner owner;
        
        [SerializeField] HealthIndicatorWidget healthIndicatorWidget;

        [Header("Health")]
        public float health = 10;
        public float currentHealth = 10;

        [Header("Speed")]
        public float speed = 1;
        public float currentSpeed = 1;
        public float speedDrop = 5.0f;

        [Header("Info")]
        public Transform targetPosition;
        public List<Character> targetEnemies = new List<Character>();


        NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (currentHealth == health)
                healthIndicatorWidget.gameObject.SetActive(false);
            else
                healthIndicatorWidget.gameObject.SetActive(true);

            if (targetEnemies.Count == 0)
                currentSpeed = speed;
            else
                currentSpeed = speed / speedDrop;

            agent.destination = targetPosition.position;
            agent.speed = currentSpeed;
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
    }
}
