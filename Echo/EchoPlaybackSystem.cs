using EchoForge.Events;
using EchoForge.ReplayStrategies;

namespace EchoForge.Echo
{
    public class EchoPlaybackSystem
    {
        private readonly GameEventBus eventBus;
        private IReplayStrategy replayStrategy;

        public EchoPlaybackSystem(GameEventBus eventBus, IReplayStrategy replayStrategy)
        {
            this.eventBus = eventBus;
            this.replayStrategy = replayStrategy;
        }

        public void SetStrategy(IReplayStrategy strategy)
        {
            replayStrategy = strategy;
        }

        public void Tick(EchoEntity echo, float previousTime, float playbackTime)
        {
            if (!echo.IsActive) return;

            replayStrategy.Replay(echo, echo.Timeline, previousTime, playbackTime, eventBus);

            if (playbackTime >= echo.Timeline.Duration)
                echo.IsActive = false;
        }
    }
}
