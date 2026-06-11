using EchoForge.Player;
using EchoForge.Utility;
using UnityEngine;

namespace EchoForge.Core.States
{
    public class RecordingState : IGameState
    {
        public void Enter(GameStateMachine context)
        {
            Debug.Log("[EchoForge] Recording started — Press R to stop.");
        }

        public void Update(GameStateMachine context, float deltaTime)
        {
            PlayerInputData input = GatherInput(context);

            if (input.RecordPressed)
            {
                context.EchoTimelineSystem.StopRecording();
                context.ChangeState(new PlayingState());
                return;
            }

            context.PlayerSystem.Update(input, deltaTime);
            context.EchoTimelineSystem.Update(context.PlayerSystem.Player, deltaTime);
            context.PuzzleSystem.Update(
                context.PlayerSystem.Player,
                context.EchoTimelineSystem.ActiveEchoes,
                deltaTime);

            if (context.PuzzleSystem.CheckLevelComplete(context.PlayerSystem.Player))
            {
                context.EchoTimelineSystem.StopRecording();
                context.ChangeState(new LevelCompleteState());
            }
        }

        public void Exit(GameStateMachine context)
        {
            Debug.Log("[EchoForge] Recording stopped.");
        }

        private PlayerInputData GatherInput(GameStateMachine context)
        {
            float x = 0f, y = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;

            float rad = context.PlayerFacingAngle * Mathf.Deg2Rad;
            float worldX = x * Mathf.Cos(rad) + y * Mathf.Sin(rad);
            float worldZ = -x * Mathf.Sin(rad) + y * Mathf.Cos(rad);

            return new PlayerInputData
            {
                MovementDirection = new VectorData(worldX, worldZ),
                InteractPressed = Input.GetKeyDown(KeyCode.E),
                RecordPressed = Input.GetKeyDown(KeyCode.R),
                DeleteEchoPressed = false
            };
        }
    }
}
