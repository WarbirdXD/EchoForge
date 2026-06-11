using EchoForge.Utility;

namespace EchoForge.Echo
{
    public class MoveCommand : IEchoCommand
    {
        public float TimeStamp { get; }
        public VectorData TargetPosition { get; }

        public MoveCommand(float timeStamp, VectorData targetPosition)
        {
            TimeStamp = timeStamp;
            TargetPosition = targetPosition;
        }

        public void Execute(EchoContext context)
        {
            context.Entity.Position = TargetPosition;
        }
    }
}
