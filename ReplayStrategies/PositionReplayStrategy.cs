using System.Collections.Generic;
using EchoForge.Echo;
using EchoForge.Events;
using EchoForge.Utility;

namespace EchoForge.ReplayStrategies
{
    public class PositionReplayStrategy : IReplayStrategy
    {
        public void Replay(EchoEntity echo, EchoTimeline timeline, float previousTime, float time, GameEventBus eventBus)
        {
            IReadOnlyList<IEchoCommand> commands = timeline.Commands;
            EchoContext context = new EchoContext(echo, eventBus);

            MoveCommand prevMove = null;
            MoveCommand nextMove = null;

            for (int i = 0; i < commands.Count; i++)
            {
                IEchoCommand cmd = commands[i];

                if (cmd is MoveCommand mc)
                {
                    if (mc.TimeStamp <= time)
                        prevMove = mc;
                    else if (nextMove == null)
                        nextMove = mc;
                }
                else if (cmd.TimeStamp > previousTime && cmd.TimeStamp <= time)
                {
                    cmd.Execute(context);
                }
            }

            VectorData from = prevMove != null ? prevMove.TargetPosition : timeline.StartPosition;

            if (prevMove != null && nextMove != null)
            {
                float range = nextMove.TimeStamp - prevMove.TimeStamp;
                float t = range > 0f ? (time - prevMove.TimeStamp) / range : 1f;
                echo.Position = VectorData.Lerp(from, nextMove.TargetPosition, t);
            }
            else
            {
                echo.Position = from;
            }
        }
    }
}
