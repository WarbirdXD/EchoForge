using System.Collections.Generic;
using EchoForge.Events;
using EchoForge.Player;
using EchoForge.ReplayStrategies;

namespace EchoForge.Echo
{
    public class EchoTimelineSystem
    {
        private readonly GameEventBus eventBus;
        private readonly TimelineRecorder recorder;
        private readonly EchoPlaybackSystem playbackSystem;
        private readonly List<EchoEntity> activeEchoes = new List<EchoEntity>();
        private readonly List<float> echoPlaybackTimes = new List<float>();

        private int nextEchoId;
        private bool isRecording;

        public const int MaxEchoes = 3;
        public IReadOnlyList<EchoEntity> ActiveEchoes => activeEchoes;
        public bool IsRecording => isRecording;

        public EchoTimelineSystem(GameEventBus eventBus)
        {
            this.eventBus = eventBus;
            recorder = new TimelineRecorder();
            playbackSystem = new EchoPlaybackSystem(eventBus, new PositionReplayStrategy());

            eventBus.Subscribe<PlayerInteractEvent>(OnPlayerInteract);
        }

        private void OnPlayerInteract(PlayerInteractEvent e)
        {
            if (isRecording)
                recorder.RecordInteract(e.Position);
        }

        public void StartRecording(PlayerEntity player)
        {
            if (activeEchoes.Count >= MaxEchoes) return;
            recorder.StartRecording(player.Position);
            isRecording = true;
        }

        public void StopRecording()
        {
            if (!isRecording) return;

            EchoTimeline timeline = recorder.StopRecording();
            isRecording = false;

            if (timeline == null || timeline.Commands.Count == 0) return;

            EchoEntity echo = new EchoEntity(nextEchoId++, timeline);
            activeEchoes.Add(echo);
            echoPlaybackTimes.Add(0f);

            eventBus.Publish(new RecordingStoppedEvent(timeline));
            eventBus.Publish(new EchoSpawnedEvent(echo));
        }

        public void DeleteOldestEcho()
        {
            if (activeEchoes.Count == 0) return;

            EchoEntity oldest = activeEchoes[0];
            activeEchoes.RemoveAt(0);
            echoPlaybackTimes.RemoveAt(0);
            eventBus.Publish(new EchoDeletedEvent(oldest));
        }

        public void Update(PlayerEntity player, float deltaTime)
        {
            if (isRecording)
                recorder.Tick(deltaTime, player);

            for (int i = activeEchoes.Count - 1; i >= 0; i--)
            {
                float previousTime = echoPlaybackTimes[i];
                echoPlaybackTimes[i] += deltaTime;
                playbackSystem.Tick(activeEchoes[i], previousTime, echoPlaybackTimes[i]);

                if (!activeEchoes[i].IsActive)
                {
                    activeEchoes[i].IsActive = true;
                    activeEchoes[i].Position = activeEchoes[i].Timeline.StartPosition;
                    echoPlaybackTimes[i] = 0f;
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < activeEchoes.Count; i++)
                eventBus.Publish(new EchoDeletedEvent(activeEchoes[i]));

            activeEchoes.Clear();
            echoPlaybackTimes.Clear();
            isRecording = false;
            nextEchoId = 0;
        }
    }
}
