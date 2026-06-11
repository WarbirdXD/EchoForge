namespace EchoForge.Events
{
    public class PlateActivatedEvent : IGameEvent
    {
        public string PlateId { get; }
        public bool IsActivated { get; }

        public PlateActivatedEvent(string plateId, bool isActivated)
        {
            PlateId = plateId;
            IsActivated = isActivated;
        }
    }
}
