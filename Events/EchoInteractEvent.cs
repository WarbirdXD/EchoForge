using EchoForge.Echo;
using EchoForge.Utility;

namespace EchoForge.Events
{
    public class EchoInteractEvent : IGameEvent
    {
        public EchoEntity Echo { get; }
        public VectorData Position { get; }

        public EchoInteractEvent(EchoEntity echo, VectorData position)
        {
            Echo = echo;
            Position = position;
        }
    }
}
