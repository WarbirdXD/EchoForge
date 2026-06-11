using EchoForge.Events;
using UnityEngine;

namespace EchoForge.Core.States
{
    public class GameOverState : IGameState
    {
        public void Enter(GameStateMachine context)
        {
            Debug.Log("[EchoForge] Game Over — Press Space to restart or Esc to quit.");
        }

        public void Update(GameStateMachine context, float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                context.EventBus.Publish(new LevelResetEvent());
                context.PlayerSystem.Reset();
                context.EchoTimelineSystem.Reset();
                context.PuzzleSystem.Reset();
                context.ChangeState(new PlayingState());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void Exit(GameStateMachine context) { }
    }
}
