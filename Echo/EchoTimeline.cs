using System.Collections.Generic;
using EchoForge.Utility;

namespace EchoForge.Echo
{
    public class EchoTimeline
    {
        private readonly List<IEchoCommand> commands = new List<IEchoCommand>();

        public float Duration { get; private set; }
        public VectorData StartPosition { get; }
        public IReadOnlyList<IEchoCommand> Commands => commands;

        public EchoTimeline(VectorData startPosition)
        {
            StartPosition = startPosition;
        }

        public void AddCommand(IEchoCommand command)
        {
            commands.Add(command);
            if (command.TimeStamp > Duration)
                Duration = command.TimeStamp;
        }

        public void Finalize(float totalDuration)
        {
            Duration = totalDuration;
        }
    }
}
