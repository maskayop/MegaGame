using UnityEngine;
using UnityEngine.AI;
using Vopere.Common;

namespace MegaGame
{
    public class Warship : BaseCharacter
    {
        [Header("Speed")]
        public short speed = 1;
        public float currentSpeed = 1;
        public short speedDrop = 5;
        public float windSpeedMinMultiplier = 1;

        [Header("FX")]
        [SerializeField] GameObject FXDestroyPrefab;
        [SerializeField] ParticleSystem FXShotLeft;
        [SerializeField] ParticleSystem FXShotRight;

        [Space(10)]
        [SerializeField] Transform FXTargetTransformLeft;
        [SerializeField] Transform FXTargetTransformRight;

        [Header("Visual")]
        [SerializeField] GameObject visualObject;
        [SerializeField] AnimationBehavior animationBehavior;

        [Header("Other")]
        [SerializeField] DestroyAfterTime destroyAfterTime;

        protected NavMeshAgent agent;
        protected Transform destinationPosition;
        protected BaseSettlement targetSettlement;

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
            OnUpdateTargetSettlementState();

            agent.destination = destinationPosition.position;
            agent.speed = currentSpeed;
        }

        protected virtual void OnUpdateTargetSettlementState()
        {
            if (owner == Owner.player)
            {
                if (targetSettlement.owner == Owner.player)
                {
                    if (targetSettlement.Island.DefenderShip)
                    {
                        if (targetSettlement.Island.DefenderShip.owner == Owner.player)
                            targetSettlement = gameController.playerOpposingPorts.antagonPort;
                    }
                    else if (!targetSettlement.Island.DefenderShip)
                        targetSettlement = gameController.playerOpposingPorts.antagonPort;
                }
            }
            else if (owner == Owner.enemy)
            {
                if (targetSettlement.owner == Owner.enemy)
                {
                    if (targetSettlement.Island.DefenderShip)
                    {
                        if (targetSettlement.Island.DefenderShip.owner == Owner.enemy)
                            targetSettlement = gameController.enemyOpposingPorts.antagonPort;
                    }
                    else if (!targetSettlement.Island.DefenderShip)
                        targetSettlement = gameController.enemyOpposingPorts.antagonPort;
                }
            }

            destinationPosition = targetSettlement.transform;
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

            PlayShotFX();
        }

        protected override void OnKilled()
        {
            if (!gameObject)
                return;

            if (FXDestroyPrefab)
                Instantiate(FXDestroyPrefab, transform.position, transform.rotation);

            if (destroyAfterTime)
            {
                Destroy(this);

                destroyAfterTime.DestroyGameObject();

                if (animationBehavior)
                    animationBehavior.Animate();

                GetCurrentHealthWidget().gameObject.SetActive(false);
            }
            else
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

        void PlayShotFX()
        {
            if (targetEnemies.Count == 0)
                return;

            if (!targetEnemies[0])
                return;

            if (Vector3.Distance(targetEnemies[0].transform.position, FXTargetTransformLeft.position) < Vector3.Distance(targetEnemies[0].transform.position, FXTargetTransformRight.position))
            {
                if (FXShotLeft)
                    FXShotLeft.Play();
            }
            else
            {
                if (FXShotRight)
                    FXShotRight.Play();
            }
        }
    }
}
