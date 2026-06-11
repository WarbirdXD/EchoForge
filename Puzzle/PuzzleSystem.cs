using System.Collections.Generic;
using EchoForge.Echo;
using EchoForge.Events;
using EchoForge.Player;
using EchoForge.Utility;

namespace EchoForge.Puzzle
{
    public class PuzzleSystem
    {
        private readonly GameEventBus eventBus;
        private readonly List<PressurePlateEntity> plates = new List<PressurePlateEntity>();
        private readonly List<DoorEntity> doors = new List<DoorEntity>();
        private readonly List<SwitchEntity> switches = new List<SwitchEntity>();
        private ExitEntity exit;

        private const float OverlapRadius = 0.6f;
        private const float InteractRadius = 1.2f;

        public PuzzleSystem(GameEventBus eventBus)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe<PlayerInteractEvent>(OnPlayerInteract);
            eventBus.Subscribe<EchoInteractEvent>(OnEchoInteract);
        }

        public void SetupLevel(
            List<PressurePlateEntity> levelPlates,
            List<DoorEntity> levelDoors,
            List<SwitchEntity> levelSwitches,
            ExitEntity levelExit)
        {
            plates.Clear();
            doors.Clear();
            switches.Clear();

            plates.AddRange(levelPlates);
            doors.AddRange(levelDoors);
            switches.AddRange(levelSwitches);
            exit = levelExit;
        }

        public void Update(PlayerEntity player, IReadOnlyList<EchoEntity> echoes, float deltaTime)
        {
            foreach (PressurePlateEntity plate in plates)
            {
                bool playerOn = IsWithinRadius(player.Position, plate.Position, plate.Radius);
                bool echoOn = false;

                foreach (EchoEntity echo in echoes)
                {
                    if (echo.IsActive && IsWithinRadius(echo.Position, plate.Position, plate.Radius))
                    {
                        echoOn = true;
                        break;
                    }
                }

                plate.SetOccupied(playerOn || echoOn);
                plate.Update(deltaTime);
            }

            foreach (DoorEntity door in doors)
                door.Update(deltaTime);

            exit?.Update(deltaTime);
        }

        public bool CheckLevelComplete(PlayerEntity player)
        {
            if (exit == null || !player.IsAlive) return false;
            return exit.CheckPlayerReached(player.Position);
        }

        private void OnPlayerInteract(PlayerInteractEvent e)
        {
            TryInteractSwitches(e.Position);
        }

        private void OnEchoInteract(EchoInteractEvent e)
        {
            TryInteractSwitches(e.Position);
        }

        private void TryInteractSwitches(VectorData position)
        {
            foreach (SwitchEntity sw in switches)
            {
                if (IsWithinRadius(position, sw.Position, InteractRadius))
                    sw.Interact();
            }
        }

        private bool IsWithinRadius(VectorData a, VectorData b, float radius)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (dx * dx + dy * dy) <= radius * radius;
        }

        public void Reset()
        {
            foreach (PressurePlateEntity plate in plates) plate.Reset();
            foreach (DoorEntity door in doors) door.Reset();
            foreach (SwitchEntity sw in switches) sw.Reset();
            exit?.Reset();
        }
    }
}
