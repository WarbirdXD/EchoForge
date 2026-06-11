namespace EchoForge.Events
{
    public class LevelCompletedEvent : IGameEvent
    {
        public float CompletionTime { get; }

        public LevelCompletedEvent(float completionTime)
        {
            CompletionTime = completionTime;
        }
    }
}
