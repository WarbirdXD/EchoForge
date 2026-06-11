using EchoForge.Player;
using EchoForge.Utility;

namespace EchoForge.Echo
{
    public class TimelineRecorder
    {
        private EchoTimeline currentTimeline;
        private float recordingTime;
        private bool isRecording;
        private VectorData lastRecordedPosition;

        public bool IsRecording => isRecording;
        public float RecordingTime => recordingTime;

        public void StartRecording(VectorData playerPosition)
        {
            currentTimeline = new EchoTimeline(playerPosition);
            recordingTime = 0f;
            lastRecordedPosition = playerPosition;
            isRecording = true;
        }

        public void Tick(float deltaTime, PlayerEntity player)
        {
            if (!isRecording) return;

            recordingTime += deltaTime;

            if (player.Position != lastRecordedPosition)
            {
                currentTimeline.AddCommand(new MoveCommand(recordingTime, player.Position));
                lastRecordedPosition = player.Position;
            }
        }

        public void RecordInteract(VectorData position)
        {
            if (!isRecording) return;
            currentTimeline.AddCommand(new InteractCommand(recordingTime, position));
        }

        public EchoTimeline StopRecording()
        {
            if (!isRecording) return null;

            currentTimeline.Finalize(recordingTime);
            isRecording = false;
            EchoTimeline result = currentTimeline;
            currentTimeline = null;
            return result;
        }
    }
}
