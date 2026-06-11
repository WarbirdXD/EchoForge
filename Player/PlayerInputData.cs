using EchoForge.Utility;

namespace EchoForge.Player
{
    public struct PlayerInputData
    {
        public VectorData MovementDirection;
        public bool InteractPressed;
        public bool RecordPressed;
        public bool DeleteEchoPressed;

        public static PlayerInputData Empty => new PlayerInputData
        {
            MovementDirection = VectorData.Zero,
            InteractPressed = false,
            RecordPressed = false,
            DeleteEchoPressed = false
        };
    }
}
