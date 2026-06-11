namespace EchoForge.Events
{
    public class DoorOpenedEvent : IGameEvent
    {
        public string DoorId { get; }
        public bool IsOpen { get; }

        public DoorOpenedEvent(string doorId, bool isOpen)
        {
            DoorId = doorId;
            IsOpen = isOpen;
        }
    }
}
