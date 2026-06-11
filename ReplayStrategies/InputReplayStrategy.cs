using System.Collections.Generic;
using EchoForge.Echo;
using EchoForge.Events;

namespace EchoForge.ReplayStrategies
{
    public class InputReplayStrategy : IReplayStrategy
    {
        private int lastExecutedIndex = -1;

        public void Replay(EchoEntity echo, EchoTimeline timeline, float previousTime, float time, GameEventBus eventBus)
        {
            IReadOnlyList<IEchoCommand> commands = timeline.Commands;
            EchoContext context = new EchoContext(echo, eventBus);

            for (int i = lastExecutedIndex + 1; i < commands.Count; i++)
            {
                if (commands[i].TimeStamp <= time)
                {
                    commands[i].Execute(context);
                    lastExecutedIndex = i;
                }
                else
                {
                    break;
                }
            }
        }

        public void Reset()
        {
            lastExecutedIndex = -1;
        }
    }
}
