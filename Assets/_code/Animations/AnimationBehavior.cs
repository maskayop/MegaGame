using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class AnimationBehavior : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] List<string> animStates = new List<string>();

        public void Animate()
        {
            short r = (short)Random.Range(0, animStates.Count);
            animator.Play(animStates[r]);
        }
    }
}
