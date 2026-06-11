using EchoForge.Events;
using EchoForge.Utility;

namespace EchoForge.Player
{
    public class PlayerSystem
    {
        private readonly GameEventBus eventBus;
        private PlayerEntity player;
        private VectorData spawnPosition;

        public PlayerEntity Player => player;
        public PlayerSystem(GameEventBus eventBus)
        {
            this.eventBus = eventBus;
            spawnPosition = new VectorData(0f, 0f);
            player = new PlayerEntity(spawnPosition);
        }

        public void SetSpawnPosition(VectorData position)
        {
            spawnPosition = position;
            player.Reset(position);
        }

        public void Update(PlayerInputData input, float deltaTime)
        {
            if (!player.IsAlive) return;

            if (input.MovementDirection != VectorData.Zero)
                player.Move(input.MovementDirection, deltaTime);

            if (input.InteractPressed)
                HandleInteract();
        }

        private void HandleInteract()
        {
            eventBus.Publish(new PlayerInteractEvent(player.Position));
        }

        public void Reset()
        {
            player.Reset(spawnPosition);
        }
    }
}
