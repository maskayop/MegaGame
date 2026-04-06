using UnityEngine;

namespace MegaGame
{
    public class UISoundPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        AudioController audioController;

        void Start()
        {
            audioController = AudioController.Instance;
        }

        public void PlayClip()
        {
            audioController?.PlayUIAudioClip(clip);
        }
    }
}
