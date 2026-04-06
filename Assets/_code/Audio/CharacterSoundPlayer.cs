using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    [RequireComponent(typeof(BaseCharacter))]
    [RequireComponent(typeof(AudioSource))]
    public class CharacterSoundPlayer : MonoBehaviour
    {
        [Header("On Start")]
        [SerializeField] List<AudioClip> startClips = new List<AudioClip>();

        [Header("On Attack")]
        [SerializeField] List<AudioClip> attackClips = new List<AudioClip>();

        [Header("On Dead")]
        [SerializeField] List<AudioClip> deadClips = new List<AudioClip>();

        AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayStartSound()
        {
            PlayRandomSound(startClips);
        }

        public void PlayAttackSound()
        {
            PlayRandomSound(attackClips);
        }

        public void PlayDeadSound()
        {
            PlayRandomSound(deadClips);
        }

        void PlayRandomSound(List<AudioClip> INlist)
        {
            if (INlist.Count == 0)
                return;

            int r = Random.Range(0, INlist.Count);
            audioSource?.PlayOneShot(INlist[r]);
        }
    }
}
