using EchoForge.Utility;

namespace EchoForge.Puzzle
{
    public interface IPuzzleObject
    {
        string Id { get; }
        VectorData Position { get; }
        void Update(float deltaTime);
        void Reset();
    }
}
