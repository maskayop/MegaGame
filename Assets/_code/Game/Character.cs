using UnityEngine;
using UnityEngine.AI;

namespace MegaGame
{
    public class Character : BaseCharacter
    {
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

        [Header("Info")]
        public Transform destinationPosition;
        public Port targetPort;

        NavMeshAgent agent;

        float currentAttackTime = 0;
        float cos = 0;

        protected override void OnAwake()
        {
            if (ObjectsManager.Instance)
                ObjectsManager.Instance.allCharacters.Add(gameObject);

            agent = GetComponent<NavMeshAgent>();
        }

        protected override void OnStart() { }

        protected override void OnInit() { }

        protected override void OnUpdate()
        {
            if (targetEnemies.Count != 0 || targetPort)
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

            if (currentHealth < 0)
                Kill();
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
            {
                targetEnemies[0].currentHealth -= damage;
                //visualObject.transform.LookAt(targetEnemies[0].transform);
            }
            else
                visualObject.transform.localRotation = Quaternion.identity;

            if (targetPort)
            {
                targetPort.currentHealth -= damage;
                //visualObject.transform.LookAt(targetPort.transform);
            }
            else
                visualObject.transform.localRotation = Quaternion.identity;

            currentAttackTime = attackDelay;

            if (FXShot)
                FXShot.Play();
        }

        void Kill()
        {
            Instantiate(FXDestroyPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
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
    }
}
