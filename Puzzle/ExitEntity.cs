using EchoForge.Events;
using EchoForge.Utility;
using UnityEngine;

namespace EchoForge.Puzzle
{
    public class ExitEntity : IPuzzleObject
    {
        private readonly GameEventBus eventBus;
        private readonly string linkedDoorId;
        private bool isUnlocked;

        public string Id { get; }
        public VectorData Position { get; }
        public float TriggerRadius { get; }
        public bool IsUnlocked => isUnlocked;

        public ExitEntity(string id, VectorData position, float triggerRadius, string linkedDoorId, GameEventBus eventBus)
        {
            Id = id;
            Position = position;
            TriggerRadius = triggerRadius;
            this.linkedDoorId = linkedDoorId;
            this.eventBus = eventBus;

            isUnlocked = string.IsNullOrEmpty(linkedDoorId);

            if (!isUnlocked)
                eventBus.Subscribe<DoorOpenedEvent>(OnDoorOpened);
        }

        private void OnDoorOpened(DoorOpenedEvent e)
        {
            if (e.DoorId != linkedDoorId) return;
            if (e.IsOpen)
            {
                isUnlocked = true;
                Debug.Log($"[EchoForge] Exit '{Id}' unlocked by door '{e.DoorId}'");
            }
        }

        public bool CheckPlayerReached(VectorData playerPosition)
        {
            if (!isUnlocked) return false;

            float dx = playerPosition.X - Position.X;
            float dy = playerPosition.Y - Position.Y;
            return (dx * dx + dy * dy) <= TriggerRadius * TriggerRadius;
        }

        public void Update(float deltaTime) { }

        public void Reset()
        {
            isUnlocked = string.IsNullOrEmpty(linkedDoorId);
        }
    }
}
