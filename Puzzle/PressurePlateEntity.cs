using EchoForge.Events;
using EchoForge.Utility;
using UnityEngine;

namespace EchoForge.Puzzle
{
    public class PressurePlateEntity : IPuzzleObject
    {
        private readonly GameEventBus eventBus;
        private bool isActivated;

        public string Id { get; }
        public VectorData Position { get; }
        public float Radius { get; }
        public bool IsActivated => isActivated;

        public PressurePlateEntity(string id, VectorData position, float radius, GameEventBus eventBus)
        {
            Id = id;
            Position = position;
            Radius = radius;
            this.eventBus = eventBus;
        }

        public void SetOccupied(bool occupied)
        {
            if (occupied == isActivated) return;

            isActivated = occupied;
            Debug.Log($"[EchoForge] Plate '{Id}' -> {(isActivated ? "ACTIVATED" : "DEACTIVATED")}");
            eventBus.Publish(new PlateActivatedEvent(Id, isActivated));
        }

        public void Update(float deltaTime) { }

        public void Reset()
        {
            isActivated = false;
        }
    }
}
