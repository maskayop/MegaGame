using UnityEngine;
using UnityEngine.AI;

namespace MegaGame
{
    public class Warship : BaseCharacter
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

            if (speedDrop == 0)
                return;

            if (targetEnemies.Count != 0)
            {
                currentSpeed = speed / speedDrop;

                if (targetEnemies[0] as Port)
                {
                    if (targetEnemies[0].GetComponent<Port>() != targetSettlement)
                        currentSpeed = speed;
                }
            }
            else
                currentSpeed = speed;

            UpdateSpeedByWind();

            if (owner == Owner.player && targetSettlement.owner == Owner.player)
                targetSettlement = gameController.playerOpposingPorts.antagonPort;
            else if (owner == Owner.enemy && targetSettlement.owner == Owner.enemy)
                targetSettlement = gameController.enemyOpposingPorts.antagonPort;
            
            destinationPosition = targetSettlement.transform;
            agent.destination = destinationPosition.position;
            agent.speed = currentSpeed;
        }

        protected override bool CanAddTargetToList(BaseCharacter targetCharacter)
        {
            if (targetCharacter as Port)
                if (targetCharacter != targetSettlement)
                    return false;

            if (targetCharacter as Village)
                if (targetCharacter != targetSettlement)
                    return false;

            if (targetCharacter as Fortress)
                if (targetCharacter != targetSettlement)
                    return false;

            return true;
        }

        protected override void OnAttack()
        {
            if (targetEnemies[0].currentHealth <= 0)
            {
                if (targetEnemies[0] as Village)
                {
                    Village targetVillage = (Village)targetEnemies[0];

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
                else if (targetEnemies[0] as Fortress)
                {
                    Fortress targetFortress = (Fortress)targetEnemies[0];

                    if (owner == Owner.player)
                    {
                        targetFortress.owner = Owner.player;
                        targetFortress.Island.owner = Owner.player;
                    }
                    else if (owner == Owner.enemy)
                    {
                        targetFortress.owner = Owner.enemy;
                        targetFortress.Island.owner = Owner.enemy;
                    }

                    targetFortress.Island.UpdateIslandState();
                    targetFortress.Init();
                    targetFortress.currentHealth = 1;
                    targetEnemies.Remove(targetFortress);

                    gameController.UpdateSettlementsLists();
                }
                else if (targetEnemies[0] as Port)
                {
                    Port port = (Port)targetEnemies[0];

                    if (port != targetSettlement)
                        return;
                }
            }

            if (FXShot)
                FXShot.Play();
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
