using EchoForge.Echo;
using EchoForge.Events;
using EchoForge.Player;
using EchoForge.Puzzle;

namespace EchoForge.Core
{
    public class GameStateMachine
    {
        private IGameState currentState;

        public GameEventBus EventBus { get; }
        public PlayerSystem PlayerSystem { get; }
        public EchoTimelineSystem EchoTimelineSystem { get; }
        public PuzzleSystem PuzzleSystem { get; }
        public float PlayerFacingAngle { get; set; }

        public GameStateMachine(
            GameEventBus eventBus,
            PlayerSystem playerSystem,
            EchoTimelineSystem echoTimelineSystem,
            PuzzleSystem puzzleSystem)
        {
            EventBus = eventBus;
            PlayerSystem = playerSystem;
            EchoTimelineSystem = echoTimelineSystem;
            PuzzleSystem = puzzleSystem;
        }

        public void ChangeState(IGameState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
        }

        public void Update(float deltaTime)
        {
            currentState?.Update(this, deltaTime);
        }
    }
}
