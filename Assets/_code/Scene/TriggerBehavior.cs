using System.Collections.Generic;
using UnityEngine;
using static MegaGame.BaseCharacter;

namespace MegaGame
{
    public class TriggerBehavior : MonoBehaviour
    {
        public float timeForTargetsUpdate = 3;
        public List<BaseCharacter> characters = new List<BaseCharacter>();

        float currentTargetsUpdateTime = 3;

        Collider coll;

        void Start()
        {
            coll = GetComponent<Collider>();
        }

        void Update()
        {
            currentTargetsUpdateTime -= Time.deltaTime;

            if (currentTargetsUpdateTime < 0)
            {
                coll.enabled = false;
                characters.Clear();
                coll.enabled = true;

                currentTargetsUpdateTime = timeForTargetsUpdate;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                if (!characters[i])
                    characters.Remove(characters[i]);
                else if (characters[i].owner == Owner.neutral)
                    characters.Remove(characters[i]);
            }
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.layer != 8 && coll.gameObject.layer != 9)
                return;

            BaseCharacter targetCharacter = coll.GetComponentInParent<BaseCharacter>();

            if (targetCharacter)
            {
                if (targetCharacter.owner != Owner.neutral)
                    characters.Add(targetCharacter);
            }
        }

        void OnTriggerExit(Collider coll)
        {
            BaseCharacter targetCharacter = coll.GetComponentInParent<BaseCharacter>();

            if (targetCharacter)
                characters.Remove(targetCharacter);
        }
    }
}
