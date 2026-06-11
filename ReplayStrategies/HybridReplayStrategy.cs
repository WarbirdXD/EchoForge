using System.Collections.Generic;
using EchoForge.Echo;
using EchoForge.Events;

namespace EchoForge.ReplayStrategies
{
    public class HybridReplayStrategy : IReplayStrategy
    {
        private readonly PositionReplayStrategy positionStrategy = new PositionReplayStrategy();
        private readonly InputReplayStrategy inputStrategy = new InputReplayStrategy();

        public void Replay(EchoEntity echo, EchoTimeline timeline, float previousTime, float time, GameEventBus eventBus)
        {
            positionStrategy.Replay(echo, timeline, previousTime, time, eventBus);
            inputStrategy.Replay(echo, timeline, previousTime, time, eventBus);
        }
    }
}
