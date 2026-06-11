using EchoForge.Events;
using EchoForge.Utility;

namespace EchoForge.Echo
{
    public class EchoContext
    {
        public EchoEntity Entity { get; }
        public GameEventBus EventBus { get; }

        public EchoContext(EchoEntity entity, GameEventBus eventBus)
        {
            Entity = entity;
            EventBus = eventBus;
        }
    }
}
