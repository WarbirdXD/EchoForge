using EchoForge.Events;
using EchoForge.Utility;

namespace EchoForge.Echo
{
    public class InteractCommand : IEchoCommand
    {
        public float TimeStamp { get; }
        public VectorData InteractPosition { get; }

        public InteractCommand(float timeStamp, VectorData interactPosition)
        {
            TimeStamp = timeStamp;
            InteractPosition = interactPosition;
        }

        public void Execute(EchoContext context)
        {
            context.EventBus.Publish(new EchoInteractEvent(context.Entity, InteractPosition));
        }
    }
}
