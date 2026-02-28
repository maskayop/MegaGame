using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class AnimationBehavior : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] List<string> animStates = new List<string>();

        [Header("Nekark")]
        [SerializeField] string sinkingState;
        [SerializeField] GameObject nekarkPrefab;

        public float timeForDestroy;

        Warship ship;

        void Start()
        {
            ship = GetComponent<Warship>();
        }

        public void Animate()
        {
            short r = (short)Random.Range(0, animStates.Count);
            animator.Play(animStates[r]);
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
