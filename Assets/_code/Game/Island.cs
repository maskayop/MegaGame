using System.Collections.Generic;

namespace MegaGame
{
    public class Island : BaseCharacter
    {
        public List<Port> ports = new List<Port>();

        protected override void OnAwake()
        {
            SetThisIslandToPorts();
        }

        protected override void OnStart()
        {
            Init();
        }

        protected override void OnInit()
        {
            if (!gameController)
                return;

            gameController.allIslands.Add(this);

            for (int i = 0; i < ports.Count; i++)
                gameController.allPorts.Add(ports[i]);
        }

        void SetThisIslandToPorts()
        {
            for (int i = 0; i < ports.Count; i++)
                ports[i].island = this;
        }
    }
}
