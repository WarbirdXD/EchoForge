using System.Collections.Generic;
using EchoForge.Events;
using EchoForge.Utility;
using UnityEngine;

namespace EchoForge.Puzzle
{
    public class DoorEntity : IPuzzleObject
    {
        private readonly GameEventBus eventBus;
        private readonly HashSet<string> linkedIds;
        private readonly HashSet<string> activeTriggers = new HashSet<string>();
        private bool isOpen;

        public string Id { get; }
        public VectorData Position { get; }
        public bool IsOpen => isOpen;

        public DoorEntity(string id, VectorData position, string[] linkedIds, GameEventBus eventBus)
        {
            Id = id;
            Position = position;
            this.linkedIds = new HashSet<string>(linkedIds);
            this.eventBus = eventBus;

            eventBus.Subscribe<PlateActivatedEvent>(OnPlateActivated);
        }

        private void OnPlateActivated(PlateActivatedEvent e)
        {
            Debug.Log($"[EchoForge] Door '{Id}' received PlateActivatedEvent from '{e.PlateId}' — linked: {linkedIds.Contains(e.PlateId)}");
            if (!linkedIds.Contains(e.PlateId)) return;

            if (e.IsActivated)
                activeTriggers.Add(e.PlateId);
            else
                activeTriggers.Remove(e.PlateId);

            bool shouldBeOpen = activeTriggers.Count > 0;
            if (shouldBeOpen == isOpen) return;

            isOpen = shouldBeOpen;
            Debug.Log($"[EchoForge] Door '{Id}' -> {(isOpen ? "OPEN" : "CLOSED")}");
            eventBus.Publish(new DoorOpenedEvent(Id, isOpen));
        }

        public void Update(float deltaTime) { }

        public void Reset()
        {
            isOpen = false;
            activeTriggers.Clear();
        }
    }
}
