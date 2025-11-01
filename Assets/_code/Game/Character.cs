using Mono.Cecil.Cil;
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

        [Header("Damage")]
        public float damage = 1.0f;
        public float attackDelay = 1.0f;

        [Header("FX")]
        [SerializeField] GameObject destroyFXprefab;


        [Header("Info")]
        public Transform targetPosition;
        public List<Character> targetEnemies = new List<Character>();
        public Port targetPort;

        NavMeshAgent agent;

        float currentAttackTime = 0;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            currentHealth = health;
        }

        void Update()
        {
            if (targetEnemies.Count != 0 || targetPort)
            {
                currentAttackTime -= Time.deltaTime;
                currentSpeed = speed / speedDrop;
            }
            else
                currentSpeed = speed;

            agent.destination = targetPosition.position;
            agent.speed = currentSpeed;

            if (currentAttackTime < 0)
                Attack();

            if (currentHealth != health)
            {
                healthIndicatorWidget.SetValue(currentHealth / health);
                healthIndicatorWidget.gameObject.SetActive(true);
            }
            else
                healthIndicatorWidget.gameObject.SetActive(false);

            if (currentHealth < 0)
                Kill();

            for (int i = 0; i < targetEnemies.Count; i++)
            {
                if (targetEnemies[i] == null)
                    targetEnemies.Remove(targetEnemies[i]);
            }
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

            Port portTarget = coll.GetComponent<Port>();

            if (portTarget)
            {
                if (owner == Owner.player)
                {
                    if (portTarget.owner == Port.Owner.enemy)
                        targetPort = portTarget;
                }
                else if (owner == Owner.enemy)
                {
                    if (portTarget.owner == Port.Owner.player)
                        targetPort = portTarget;
                }
            }
        }

        void OnTriggerExit(Collider coll)
        {
            Character targetCharacter = coll.GetComponentInParent<Character>();

            if (targetCharacter)
                targetEnemies.Remove(targetCharacter);

            Port portTarget = coll.GetComponent<Port>();

            if (portTarget)
                targetPort = null;
        }

        void Attack()
        {
            if (targetEnemies.Count != 0)
                targetEnemies[0].currentHealth -= damage;

            if (targetPort)
                targetPort.currentHealth -= damage;

            currentAttackTime = attackDelay;
        }

        void Kill()
        {
            Instantiate(destroyFXprefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
