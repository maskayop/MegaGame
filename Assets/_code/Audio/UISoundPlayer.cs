using UnityEngine;

namespace MegaGame.UI
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
            audioController = AudioController.Instance;
            audioController.PlayUIAudioClip(clip);
        }

        public void PlayEndGameMusic()
        {
            audioController = AudioController.Instance;
            audioController.PlayUIAudioClip(clip);

            if (audioController.GetCurrentMusicPack() != 0)
            {
                audioController.StopPlayingMusic();
                audioController.SetCurrentMusicPack(0);
                audioController.PlayNextMusicClip();
            }
        }

        public void PlayEndCampaignMusic(bool isVictory)
        {
            audioController = AudioController.Instance;

            if (isVictory)
            {
                if (audioController.GetCurrentMusicPack() != 2)
                {
                    audioController.StopPlayingMusic();
                    audioController.SetCurrentMusicPack(2);
                    audioController.PlayNextMusicClip();
                }
            }
            else
            {
                if (audioController.GetCurrentMusicPack() != 3)
                {
                    audioController.StopPlayingMusic();
                    audioController.SetCurrentMusicPack(3);
                    audioController.PlayNextMusicClip();
                }
            }
        }
    }
}
