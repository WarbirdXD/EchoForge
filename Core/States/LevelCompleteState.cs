using EchoForge.Events;
using UnityEngine;

namespace EchoForge.Core.States
{
    public class LevelCompleteState : IGameState
    {
        private float timer;

        public void Enter(GameStateMachine context)
        {
            timer = 0f;
            context.EventBus.Publish(new LevelCompletedEvent(Time.time));
            Debug.Log("[EchoForge] Level Complete! Press Space to restart.");
        }

        public void Update(GameStateMachine context, float deltaTime)
        {
            timer += deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
                RestartLevel(context);
        }

        public void Exit(GameStateMachine context) { }

        private void RestartLevel(GameStateMachine context)
        {
            context.PlayerSystem.Reset();
            context.EventBus.Publish(new LevelResetEvent());
            context.EchoTimelineSystem.Reset();
            context.PuzzleSystem.Reset();
            context.ChangeState(new PlayingState());
        }
    }
}
