using UnityEngine;
using UnityEngine.AI;

namespace MegaGame
{
    public class Character : BaseCharacter
    {
        [Header("Money")]
        public short maintenance = 1;

        [Header("Widgets")]
        [SerializeField] HealthIndicatorWidget healthIndicatorWidget;

        [Header("Speed")]
        public float speed = 1;
        public float currentSpeed = 1;
        public float speedDrop = 5;
        public float windSpeedMinMultiplier = 1;

        [Header("FX")]
        [SerializeField] GameObject FXDestroyPrefab;
        [SerializeField] ParticleSystem FXShot;

        [Header("Visual")]
        [SerializeField] GameObject visualObject;

        [HideInInspector]
        public Transform destinationPosition;

        NavMeshAgent agent;

        float currentAttackTime = 0;
        float cos = 0;

        int currentDay = 0;

        protected override void OnAwake()
        {
            if (ObjectsManager.Instance)
                ObjectsManager.Instance.allCharacters.Add(gameObject);

            agent = GetComponent<NavMeshAgent>();
        }

        protected override void OnStart()
        {
            Init();
        }

        protected override void OnInit()
        {
            currentDay = globalTime.currentDay;
        }

        protected override void OnUpdate()
        {
            if (gameController.gameState == GameController.GameState.menu)
            {
                Destroy(gameObject);
                return;
            }

            if (targetEnemies.Count != 0)
            {
                currentAttackTime -= Time.deltaTime;
                currentSpeed = speed / speedDrop;
            }
            else
                currentSpeed = speed;

            if (WindController.Instance)
            {
                cos = WindController.Instance.currentRotation.eulerAngles.y - transform.rotation.eulerAngles.y;
                cos = Mathf.Cos(Mathf.Deg2Rad * cos);
                cos = (cos + 1) / 2;

                if (cos <= windSpeedMinMultiplier)
                    cos = windSpeedMinMultiplier;

                currentSpeed *= cos;
                currentSpeed *= WindController.Instance.currentStrength;
            }

            agent.destination = destinationPosition.position;
            agent.speed = currentSpeed;

            if (currentAttackTime < 0)
                Attack();

            UpdateHealthWidget();
            UpdateTargets();
            UpdateProperties();

            if (currentHealth < 0)
                Kill();

            /*
            if (targetEnemies.Count != 0)
            {
                visualObject.transform.LookAt(targetEnemies[0].transform);
                visualObject.transform.Rotate(new Vector3(0, 45, 0));
            }
            else
                visualObject.transform.localRotation = Quaternion.identity;
            */
        }

        void OnTriggerEnter(Collider coll)
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

        void OnTriggerExit(Collider coll)
        {
            BaseCharacter targetCharacter = coll.GetComponentInParent<BaseCharacter>();

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

            if (targetEnemies[0].currentHealth <= 0)
            {
                if (targetEnemies[0] as Village)
                {
                    if (owner == Owner.player)
                        targetEnemies[0].owner = Owner.player;
                    else if (owner == Owner.enemy)
                        targetEnemies[0].owner = Owner.enemy;

                    targetEnemies.Remove(targetEnemies[0]);
                }

                if (owner == Owner.player)
                    destinationPosition = gameController.currentEnemyPort.transform;
                else if (owner == Owner.enemy)
                    destinationPosition = gameController.currentPlayerPort.transform;
            }
        }

        void Kill()
        {
            Instantiate(FXDestroyPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            ObjectsManager.Instance.allCharacters.Remove(gameObject);
        }

        void UpdateHealthWidget()
        {
            if (currentHealth != health)
            {
                healthIndicatorWidget.SetValue(currentHealth / health);
                healthIndicatorWidget.gameObject.SetActive(true);
            }
            else
                healthIndicatorWidget.gameObject.SetActive(false);
        }

        void UpdateTargets()
        {
            for (int i = 0; i < targetEnemies.Count; i++)
            {
                if (targetEnemies[i] == null)
                    targetEnemies.Remove(targetEnemies[i]);
            }
        }

        void UpdateProperties()
        {
            if (globalTime.currentDay != currentDay)
            {
                currentDay = globalTime.currentDay;
            }
        }
    }
}
