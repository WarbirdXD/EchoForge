using EchoForge.Events;
using EchoForge.Utility;

namespace EchoForge.Puzzle
{
    public class SwitchEntity : IPuzzleObject
    {
        private readonly GameEventBus eventBus;
        private bool isActivated;

        public string Id { get; }
        public VectorData Position { get; }
        public float InteractRadius { get; }
        public bool IsActivated => isActivated;

        public SwitchEntity(string id, VectorData position, float interactRadius, GameEventBus eventBus)
        {
            Id = id;
            Position = position;
            InteractRadius = interactRadius;
            this.eventBus = eventBus;
        }

        public void Interact()
        {
            isActivated = !isActivated;
            eventBus.Publish(new PlateActivatedEvent(Id, isActivated));
        }

        public void Update(float deltaTime) { }

        public void Reset()
        {
            isActivated = false;
        }
    }
}
