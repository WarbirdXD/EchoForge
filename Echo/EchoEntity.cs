using EchoForge.Utility;

namespace EchoForge.Echo
{
    public class EchoEntity
    {
        public int Id { get; }
        public VectorData Position { get; set; }
        public bool IsActive { get; set; }
        public EchoTimeline Timeline { get; }

        public EchoEntity(int id, EchoTimeline timeline)
        {
            Id = id;
            Timeline = timeline;
            Position = timeline.StartPosition;
            IsActive = true;
        }
    }
}
