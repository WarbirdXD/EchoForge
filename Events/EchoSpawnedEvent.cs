using EchoForge.Echo;

namespace EchoForge.Events
{
    public class EchoSpawnedEvent : IGameEvent
    {
        public EchoEntity Echo { get; }

        public EchoSpawnedEvent(EchoEntity echo)
        {
            Echo = echo;
        }
    }
}
