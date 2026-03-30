using UnityEngine;
using UnityEngine.AI;
using Vopere.Common;

namespace MegaGame
{
    public class Warship : BaseCharacter
    {
        [Header("Speed")]
        public int speed = 1;
        public float currentSpeed = 1;
        public int speedDrop = 5;
        public float windSpeedMinMultiplier = 1;

        [SerializeField] protected float distanceForPirateLair = 5;

        [Header("FX")]
        [SerializeField] GameObject FXDestroyPrefab;
        [SerializeField] ParticleSystem FXShotLeft;
        [SerializeField] ParticleSystem FXShotRight;

        [Space(10)]
        [SerializeField] Transform FXTargetTransformLeft;
        [SerializeField] Transform FXTargetTransformRight;

        [Header("Visual")]
        [SerializeField] protected GameObject visualObject;

        protected NavMeshAgent agent;
        protected Transform destinationPosition;
        protected BaseSettlement targetSettlement;

        AnimationBehavior animationBehavior;
        DestroyAfterTime destroyAfterTime;

        float cos = 0;

        bool isKilled = false;
        public bool IsKilled { get { return isKilled; } }

        bool killedOnStart = false;
        public bool KilledOnStart { get { return killedOnStart; } set { killedOnStart = value; } }

        bool killedByNekark = false;
        public bool KilledByNekark { get { return killedByNekark; } set { killedByNekark = value; } }

        bool killedByNafaivel = false;
        public bool KilledByNafaivel { get { return killedByNafaivel; } set { killedByNafaivel = value; } }

        protected override void OnAwake()
        {
            if (ObjectsManager.Instance)
                ObjectsManager.Instance.allShips.Add(gameObject);

            agent = GetComponent<NavMeshAgent>();
        }

        protected override void OnInit()
        {
            animationBehavior = GetComponent<AnimationBehavior>();
            destroyAfterTime = GetComponent<DestroyAfterTime>();
        }

        protected override void OnUpdate()
        {
            if (isKilled)
                return;

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

            if (destinationPosition)
                agent.destination = destinationPosition.position;
            else
                agent.destination = Vector3.zero;

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
                else if (targetSettlement.owner == Owner.neutral)
                {
                    if (targetSettlement as PirateLair)
                    {
                        if (targetSettlement.IsCaptured)
                            targetSettlement = gameController.playerOpposingPorts.antagonPort;
                        else if (Vector3.Distance(transform.position, targetSettlement.transform.position) <= distanceForPirateLair)
                            targetSettlement = gameController.playerOpposingPorts.antagonPort;
                    }
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
                else if (targetSettlement.owner == Owner.neutral)
                {
                    if (targetSettlement as PirateLair)
                    {
                        if (targetSettlement.IsCaptured)
                            targetSettlement = gameController.enemyOpposingPorts.antagonPort;
                        else if (Vector3.Distance(transform.position, targetSettlement.transform.position) <= distanceForPirateLair)
                        {
                            targetSettlement.isCaptured = true;
                            targetSettlement = gameController.enemyOpposingPorts.antagonPort;
                        }
                    }
                }
            }
            else if (owner == Owner.neutral)
                return;

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

            if (targetCharacter as PirateLair)
                if (targetCharacter.GetComponent<PirateLair>().IsCaptured)
                    return false;

            return true;
        }

        protected override void OnAttack()
        {
            if (targetEnemies[0].currentHealth <= 0)
            {
                if (targetEnemies[0] as Village || targetEnemies[0] as Fortress)
                {
                    BaseSettlement target = (BaseSettlement)targetEnemies[0];

                    target.owner = owner;
                    target.Island.owner = owner;
                    target.Island.UpdateIslandState();
                    target.Init();
                    target.currentHealth = 1;
                    targetEnemies.Remove(target);

                    gameController.UpdateSettlementsLists();
                }
                else if (targetEnemies[0] as PirateLair)
                {
                    PirateLair target = (PirateLair)targetEnemies[0];

                    target.Kill();
                    targetEnemies.Remove(target);
                }
                else if (targetEnemies[0] as Port)
                {
                    if ((Port)targetEnemies[0] != targetSettlement)
                        return;
                }
            }

            PlayShotFX();
        }

        protected override void OnKilled()
        {
            if (isKilled)
                return;

            if (!gameObject)
                return;

            if (FXDestroyPrefab && !KilledByNekark && !killedByNafaivel && !KilledOnStart)
                Instantiate(FXDestroyPrefab, transform.position, transform.rotation);

            if (destroyAfterTime && !KilledByNekark && !killedByNafaivel && !KilledOnStart)
            {
                destroyAfterTime.DestroyGameObject();

                if (animationBehavior)
                    animationBehavior.Animate();
            }
            else if (destroyAfterTime && KilledByNekark)
            {
                destroyAfterTime.DestroyGameObjectAfterTime(animationBehavior.timeForDestroy);

                if (animationBehavior)
                    animationBehavior.AnimateNekark();
            }
            else if (destroyAfterTime && killedByNafaivel)
            {
                destroyAfterTime.DestroyGameObjectAfterTime(animationBehavior.timeForDestroy);

                if (animationBehavior)
                    animationBehavior.AnimateNafaivel();
            }
            else
                Destroy(gameObject);

            Destroy(this);
            GetCurrentHealthWidget().gameObject.SetActive(false);

            ObjectsManager.Instance.allShips.Remove(gameObject);

            isKilled = true;
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

        public void SetDestinationSettlementPosition(BaseSettlement destinationSettlement)
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

            if (Vector3.Distance(targetEnemies[0].transform.position, FXTargetTransformLeft.position) <
                Vector3.Distance(targetEnemies[0].transform.position, FXTargetTransformRight.position))
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

        public Transform GetVisualObjectTransform()
        {
            return visualObject.transform;
        }

        public AnimationBehavior GetAnimationBehavior()
        {
            return animationBehavior;
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            return agent;
        }
    }
}
