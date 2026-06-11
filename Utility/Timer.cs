namespace EchoForge.Utility
{
    public class Timer
    {
        private float elapsed;
        private float duration;
        private bool running;

        public float Elapsed => elapsed;
        public bool IsRunning => running;
        public bool IsFinished => running && elapsed >= duration;

        public void Start(float durationSeconds)
        {
            duration = durationSeconds;
            elapsed = 0f;
            running = true;
        }

        public void Reset()
        {
            elapsed = 0f;
            running = false;
        }

        public void Tick(float deltaTime)
        {
            if (!running) return;
            elapsed += deltaTime;
        }
    }
}
