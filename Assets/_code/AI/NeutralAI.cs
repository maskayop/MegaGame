using UnityEngine;

namespace MegaGame
{
    public class NeutralAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;

        float currentDecisionTime = 0;

        GameController gameController;
        ObjectsManager objectsManager;

        void Start()
        {
            gameController = GameController.Instance;
            objectsManager = ObjectsManager.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            currentDecisionTime -= Time.deltaTime;

            if (currentDecisionTime <= 0)
            {
                currentDecisionTime = timeForDecision;
                MakeDecision();
            }
        }

        void MakeDecision()
        {

        }
    }
}
