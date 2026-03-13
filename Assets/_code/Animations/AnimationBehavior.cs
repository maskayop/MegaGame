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
        [SerializeField] string nekarkSinkingState;
        [SerializeField] GameObject nekarkPrefab;

        [Header("Nekark")]
        [SerializeField] string nafaivelSinkingState;
        [SerializeField] GameObject nafaivelPrefab;

        [Header("Destroy")]
        public float timeForDestroy;

        Warship ship;

        void Start()
        {
            ship = GetComponent<Warship>();
        }

        public void Animate()
        {
            int r = Random.Range(0, destroyAnimStates.Count);
            animator.Play(destroyAnimStates[r]);
        }

        public void AnimateNekark()
        {
            if (!nekarkPrefab)
                return;

            animator.Play(nekarkSinkingState);
            Instantiate(nekarkPrefab, ship.GetVisualObjectTransform());
        }

        public void AnimateNafaivel()
        {
            if (!nafaivelPrefab)
                return;

            animator.Play(nafaivelSinkingState);
            Instantiate(nafaivelPrefab, ship.GetVisualObjectTransform().position, ship.GetVisualObjectTransform().rotation);
        }

        public bool CanBeAnimatedByNekark()
        {
            if (nekarkPrefab)
                return true;
            else
                return false;
        }

        public bool CanBeAnimatedByNafaivel()
        {
            if (nafaivelPrefab)
                return true;
            else
                return false;
        }
    }
}
