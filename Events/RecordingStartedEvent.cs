namespace EchoForge.Events
{
    public class RecordingStartedEvent : IGameEvent
    {
        public float StartTime { get; }

        public RecordingStartedEvent(float startTime)
        {
            StartTime = startTime;
        }
    }
}
