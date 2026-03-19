using UnityEngine;

namespace MegaGame.UI
{
    public class UIDistanceForTargetButton : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] string animationState;

        ScenePrefabsManager scenePrefabsManager;

        void Start()
        {
            scenePrefabsManager = ScenePrefabsManager.Instance;
        }

        public void OnClickAction()
        {
            animator.Play(animationState);
            scenePrefabsManager.ShowConstantDistanceCircle();
        }
    }
}
