using EchoForge.Echo;

namespace EchoForge.Events
{
    public class EchoDeletedEvent : IGameEvent
    {
        public EchoEntity Echo { get; }

        public EchoDeletedEvent(EchoEntity echo)
        {
            Echo = echo;
        }
    }
}
