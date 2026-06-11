using UnityEngine;

namespace EchoForge.Core.States
{
    public class StartupState : IGameState
    {
        private float timer;
        private const float StartupDuration = 2f;

        public void Enter(GameStateMachine context)
        {
            timer = 0f;
            Debug.Log("[EchoForge] Startup — Press Space to begin.");
        }

        public void Update(GameStateMachine context, float deltaTime)
        {
            timer += deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) || timer >= StartupDuration)
                context.ChangeState(new PlayingState());
        }

        public void Exit(GameStateMachine context)
        {
            Debug.Log("[EchoForge] Startup complete.");
        }
    }
}
