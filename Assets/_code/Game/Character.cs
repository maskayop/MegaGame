using UnityEngine;
using UnityEngine.AI;

namespace MegaGame
{
    public class Character : BaseCharacter
    {
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

        NavMeshAgent agent;
        Transform destinationPosition;

        float cos = 0;

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
                currentSpeed = speed / speedDrop;
            else
                currentSpeed = speed;

            UpdateSpeedByWind();

            agent.destination = destinationPosition.position;
            agent.speed = currentSpeed;

            UpdateTargets();
        }

        protected override void OnAttack()
        {
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

        protected override void OnKilled()
        {
            Instantiate(FXDestroyPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            ObjectsManager.Instance.allCharacters.Remove(gameObject);
        }

        void UpdateTargets()
        {
            for (int i = 0; i < targetEnemies.Count; i++)
            {
                if (targetEnemies[i] == null)
                    targetEnemies.Remove(targetEnemies[i]);
            }
        }

        void UpdateSpeedByWind()
        {
            if (!WindController.Instance)
                return;

            cos = WindController.Instance.currentRotation.eulerAngles.y - transform.rotation.eulerAngles.y;
            cos = Mathf.Cos(Mathf.Deg2Rad * cos);
            cos = (cos + 1) / 2;

            if (cos <= windSpeedMinMultiplier)
                cos = windSpeedMinMultiplier;

            currentSpeed *= cos;
            currentSpeed *= WindController.Instance.currentStrength;
        }

        public void SetDestinationPosition(Transform destination)
        {
            destinationPosition = destination;
        }
    }
}
