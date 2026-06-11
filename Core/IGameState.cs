namespace EchoForge.Core
{
    public interface IGameState
    {
        void Enter(GameStateMachine context);
        void Update(GameStateMachine context, float deltaTime);
        void Exit(GameStateMachine context);
    }
}
