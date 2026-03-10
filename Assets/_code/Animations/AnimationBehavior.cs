using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class AnimationBehavior : MonoBehaviour
    {
        [SerializeField] Animator animator;

        [Space(20)]
        [SerializeField] List<string> destroyAnimStates = new List<string>();

        [Header("Nekark")]
        [SerializeField] string sinkingState;
        [SerializeField] GameObject nekarkPrefab;

        [Header("Destroy")]
        public float timeForDestroy;

        Warship ship;

        void Start()
        {
            ship = GetComponent<Warship>();
        }

        public void Animate()
        {
            short r = (short)Random.Range(0, destroyAnimStates.Count);
            animator.Play(destroyAnimStates[r]);
        }

        public void AnimateNekark()
        {
            if (!nekarkPrefab)
                return;

            animator.Play(sinkingState);
            Instantiate(nekarkPrefab, ship.GetVisualObjectTransform());
        }

        public bool CanBeAnimatedByNekark()
        {
            if (nekarkPrefab)
                return true;
            else
                return false;
        }
    }
}
