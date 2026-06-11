using EchoForge.Echo;

namespace EchoForge.Events
{
    public class RecordingStoppedEvent : IGameEvent
    {
        public EchoTimeline Timeline { get; }

        public RecordingStoppedEvent(EchoTimeline timeline)
        {
            Timeline = timeline;
        }
    }
}
