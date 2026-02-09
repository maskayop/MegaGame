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
        BaseSettlement targetSettlement;

        float cos = 0;

        protected override void OnAwake()
        {
            if (ObjectsManager.Instance)
                ObjectsManager.Instance.allCharacters.Add(gameObject);

            agent = GetComponent<NavMeshAgent>();
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
                if (speedDrop != 0)
                    currentSpeed = speed / speedDrop;
                else
                    currentSpeed = speed;
            }
            else
                currentSpeed = speed;

            UpdateSpeedByWind();

            if (owner == Owner.player && targetSettlement.owner == Owner.player)
                targetSettlement = gameController.currentEnemyPort;
            else if (owner == Owner.enemy && targetSettlement.owner == Owner.enemy)
                targetSettlement = gameController.currentPlayerPort;
            
            destinationPosition = targetSettlement.transform;
            agent.destination = destinationPosition.position;
            agent.speed = currentSpeed;
        }

        protected override void OnAttack()
        {
            if (FXShot)
                FXShot.Play();

            if (targetEnemies[0].currentHealth <= 0)
            {
                if (targetEnemies[0] as Village)
                {
                    Village targetVillage = targetEnemies[0].GetComponent<Village>();

                    if (owner == Owner.player)
                    {
                        targetVillage.owner = Owner.player;
                        targetVillage.Island.owner = Owner.player;
                    }
                    else if (owner == Owner.enemy)
                    {
                        targetVillage.owner = Owner.enemy;
                        targetVillage.Island.owner = Owner.enemy;
                    }

                    targetVillage.Island.UpdateIslandState();
                    targetVillage.Init();
                    targetVillage.currentHealth = 1;
                    targetEnemies.Remove(targetVillage);

                    gameController.UpdateSettlementsLists();
                }
            }
        }

        protected override void OnKilled()
        {
            Instantiate(FXDestroyPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            ObjectsManager.Instance.allCharacters.Remove(gameObject);
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

        public void SetDestinationPosition(BaseSettlement destinationSettlement)
        {
            targetSettlement = destinationSettlement;
            destinationPosition = targetSettlement.transform;
        }
    }
}
