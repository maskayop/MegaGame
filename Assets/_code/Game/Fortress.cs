using UnityEngine;

namespace MegaGame
{
    public class Fortress : BaseSettlement
    {
        [Header("FX")]
        [SerializeField] ParticleSystem FXShot;

        protected override void OnAttack()
        {
            if (FXShot)
                FXShot.Play();
        }
    }
}
