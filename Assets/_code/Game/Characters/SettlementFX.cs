using UnityEngine;

namespace MegaGame
{
    [RequireComponent(typeof(BaseSettlement))]
    public class SettlementFX : MonoBehaviour
    {
        [Header("FX")]
        [SerializeField] ParticleSystem FXShotLeft;
        [SerializeField] ParticleSystem FXShotRight;

        [Space(10)]
        [SerializeField] ParticleSystem FXFortShotLeft;
        [SerializeField] ParticleSystem FXFortShotRight;

        [Space(10)]
        [SerializeField] Transform FXTargetTransformLeft;
        [SerializeField] Transform FXTargetTransformRight;

        SettlementConstructions settlementConstructions;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            settlementConstructions = GetComponent<SettlementConstructions>();
        }

        public void PlayShotFX(Vector3 targetPosition)
        {
            if (Vector3.Distance(targetPosition, FXTargetTransformLeft.position) < Vector3.Distance(targetPosition, FXTargetTransformRight.position))
            {
                if (FXShotLeft)
                    FXShotLeft.Play();

                if (settlementConstructions && settlementConstructions.fortIsBuilt)
                    if (FXFortShotLeft)
                        FXFortShotLeft.Play();
            }
            else
            {
                if (FXShotRight)
                    FXShotRight.Play();

                if (settlementConstructions && settlementConstructions.fortIsBuilt)
                    if (FXFortShotRight)
                        FXFortShotRight.Play();
            }
        }
    }
}
