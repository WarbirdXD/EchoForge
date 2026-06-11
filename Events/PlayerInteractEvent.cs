using EchoForge.Utility;

namespace EchoForge.Events
{
    public class PlayerInteractEvent : IGameEvent
    {
        public VectorData Position { get; }

        public PlayerInteractEvent(VectorData position)
        {
            Position = position;
        }
    }
}
