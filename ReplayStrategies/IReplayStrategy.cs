using EchoForge.Echo;
using EchoForge.Events;

namespace EchoForge.ReplayStrategies
{
    public interface IReplayStrategy
    {
        void Replay(EchoEntity echo, EchoTimeline timeline, float previousTime, float time, GameEventBus eventBus);
    }
}
