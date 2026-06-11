using EchoForge.Utility;

namespace EchoForge.Player
{
    public class PlayerEntity
    {
        public VectorData Position { get; set; }
        public float Speed { get; set; }
        public bool IsAlive { get; set; }

        public PlayerEntity(VectorData startPosition, float speed = 5f)
        {
            Position = startPosition;
            Speed = speed;
            IsAlive = true;
        }

        public void Move(VectorData direction, float deltaTime)
        {
            Position = Position + direction * (Speed * deltaTime);
        }

        public void Reset(VectorData startPosition)
        {
            Position = startPosition;
            IsAlive = true;
        }
    }
}
