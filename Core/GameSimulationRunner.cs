using System.Collections.Generic;
using EchoForge.Core.States;
using EchoForge.Echo;
using EchoForge.Events;
using EchoForge.Player;
using EchoForge.Puzzle;
using EchoForge.Utility;
using UnityEngine;

namespace EchoForge.Core
{
    [System.Serializable]
    public class PressurePlateConfig
    {
        public Transform Transform;
        public string Id = "plate_01";
        public float Radius = 1.0f;
    }

    [System.Serializable]
    public class SwitchConfig
    {
        public Transform Transform;
        public string Id = "switch_01";
        public float InteractRadius = 1.2f;
    }

    [System.Serializable]
    public class DoorConfig
    {
        public Transform Transform;
        public string Id = "door_01";
        [Tooltip("IDs of plates/switches that open this door")]
        public string[] LinkedTriggerIds;
        [Tooltip("Where the door moves when open. Place an empty GameObject at the open position and drag it here.")]
        public Transform OpenPositionTransform;
        [Tooltip("Speed at which the door slides to its open/closed position")]
        public float MoveSpeed = 3f;
    }

    [System.Serializable]
    public class ExitConfig
    {
        public Transform Transform;
        public string Id = "exit_01";
        public float TriggerRadius = 1f;
        [Tooltip("Door that must be open to use this exit. Leave blank to always allow exit.")]
        public string LinkedDoorId;
    }

    public class GameSimulationRunner : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float pitchClamp = 80f;

        [Header("Echoe")]
        [SerializeField] private GameObject echoPrefab;

        [Header("Puzzle Objects")]
        [SerializeField] private PressurePlateConfig[] pressurePlates;
        [SerializeField] private SwitchConfig[] switches;
        [SerializeField] private DoorConfig[] doors;
        [SerializeField] private ExitConfig exit;

        private GameStateMachine gameStateMachine;
        private PlayerSystem playerSystem;
        private EchoTimelineSystem echoTimelineSystem;
        private PuzzleSystem puzzleSystem;
        private GameEventBus eventBus;

        private readonly List<GameObject> echoObjects = new List<GameObject>();
        private readonly Dictionary<string, Collider> doorColliders = new Dictionary<string, Collider>();

        private struct DoorVisual
        {
            public Transform Transform;
            public Vector3 ClosedPosition;
            public Vector3 OpenPosition;
            public float MoveSpeed;
            public bool IsOpen;
        }
        private readonly List<DoorVisual> doorVisuals = new List<DoorVisual>();
        private readonly List<(Vector3 pos, float radius)> debugPlateZones = new List<(Vector3, float)>();
        private readonly List<(Vector3 pos, float radius)> debugSwitchZones = new List<(Vector3, float)>();
        private (Vector3 pos, float radius) debugExitZone;

        private CharacterController characterController;
        private Vector3 playerVelocity;
        private float playerYHeight;
        private float currentYaw;
        private float currentPitch;

        private const float Gravity = -20f;

        private void Start()
        {
            eventBus = new GameEventBus();

            playerSystem = new PlayerSystem(eventBus);
            echoTimelineSystem = new EchoTimelineSystem(eventBus);
            puzzleSystem = new PuzzleSystem(eventBus);

            SetupLevel();

            gameStateMachine = new GameStateMachine(
                eventBus,
                playerSystem,
                echoTimelineSystem,
                puzzleSystem);

            eventBus.Subscribe<EchoSpawnedEvent>(OnEchoSpawned);
            eventBus.Subscribe<EchoDeletedEvent>(OnEchoDeleted);
            eventBus.Subscribe<DoorOpenedEvent>(OnDoorOpened);
            eventBus.Subscribe<LevelResetEvent>(OnLevelReset);

            doorVisuals.Clear();
            if (doors != null)
            {
                foreach (DoorConfig cfg in doors)
                {
                    if (cfg?.Transform == null || string.IsNullOrEmpty(cfg.Id)) continue;

                    Collider col = cfg.Transform.GetComponent<Collider>();
                    if (col != null) doorColliders[cfg.Id] = col;

                    if (cfg.OpenPositionTransform != null)
                    {
                        doorVisuals.Add(new DoorVisual
                        {
                            Transform    = cfg.Transform,
                            ClosedPosition = cfg.Transform.position,
                            OpenPosition   = cfg.OpenPositionTransform.position,
                            MoveSpeed    = cfg.MoveSpeed,
                            IsOpen       = false
                        });
                    }
                }
            }

            if (playerTransform != null)
            {
                playerYHeight = playerTransform.position.y;
                currentYaw = playerTransform.eulerAngles.y;
                characterController = playerTransform.GetComponent<CharacterController>();
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            gameStateMachine.ChangeState(new StartupState());
        }

        private void Update()
        {
            HandleMouseLook();

            gameStateMachine.PlayerFacingAngle = currentYaw;

            float deltaTime = Time.deltaTime;
            gameStateMachine.Update(deltaTime);

            ApplyCharacterControllerMovement(deltaTime);
            UpdateDoorVisuals(deltaTime);
            SyncTransforms();
        }

        private void HandleMouseLook()
        {
            if (playerTransform == null) return;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            currentYaw += mouseX;
            currentPitch -= mouseY;
            currentPitch = Mathf.Clamp(currentPitch, -pitchClamp, pitchClamp);

            playerTransform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);

            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        }

        private void SetupLevel()
        {
            VectorData playerSpawn = playerTransform != null
                ? ToVec(playerTransform.position)
                : VectorData.Zero;
            playerSystem.SetSpawnPosition(playerSpawn);
            Debug.Log($"[EchoForge] Player spawn: {playerSpawn}");

            debugPlateZones.Clear();
            debugSwitchZones.Clear();

            var plateEntities = new List<PressurePlateEntity>();
            if (pressurePlates != null)
            {
                foreach (PressurePlateConfig cfg in pressurePlates)
                {
                    if (cfg == null || string.IsNullOrEmpty(cfg.Id)) continue;
                    VectorData pos = cfg.Transform != null ? ToVec(cfg.Transform.position) : VectorData.Zero;
                    plateEntities.Add(new PressurePlateEntity(cfg.Id, pos, cfg.Radius, eventBus));
                    debugPlateZones.Add((cfg.Transform != null ? cfg.Transform.position : Vector3.zero, cfg.Radius));
                    Debug.Log($"[EchoForge] Plate '{cfg.Id}' at world({pos.X:F2}, {pos.Y:F2}) radius={cfg.Radius}");
                }
            }

            if (plateEntities.Count == 0)
                Debug.LogWarning("[EchoForge] No pressure plates configured! Add entries to the Pressure Plates array in the Inspector.");

            var switchEntities = new List<SwitchEntity>();
            if (switches != null)
            {
                foreach (SwitchConfig cfg in switches)
                {
                    if (cfg == null || string.IsNullOrEmpty(cfg.Id)) continue;
                    VectorData pos = cfg.Transform != null ? ToVec(cfg.Transform.position) : VectorData.Zero;
                    switchEntities.Add(new SwitchEntity(cfg.Id, pos, cfg.InteractRadius, eventBus));
                    debugSwitchZones.Add((cfg.Transform != null ? cfg.Transform.position : Vector3.zero, cfg.InteractRadius));
                    Debug.Log($"[EchoForge] Switch '{cfg.Id}' at world({pos.X:F2}, {pos.Y:F2}) interactRadius={cfg.InteractRadius}");
                }
            }

            var doorEntities = new List<DoorEntity>();
            if (doors != null)
            {
                foreach (DoorConfig cfg in doors)
                {
                    if (cfg == null || string.IsNullOrEmpty(cfg.Id)) continue;
                    VectorData pos = cfg.Transform != null ? ToVec(cfg.Transform.position) : VectorData.Zero;
                    string[] linked = cfg.LinkedTriggerIds ?? new string[0];
                    doorEntities.Add(new DoorEntity(cfg.Id, pos, linked, eventBus));
                }
            }

            ExitEntity exitEntity = null;
            if (exit != null && !string.IsNullOrEmpty(exit.Id))
            {
                VectorData pos = exit.Transform != null ? ToVec(exit.Transform.position) : VectorData.Zero;
                exitEntity = new ExitEntity(exit.Id, pos, exit.TriggerRadius, exit.LinkedDoorId, eventBus);
                debugExitZone = (exit.Transform != null ? exit.Transform.position : Vector3.zero, exit.TriggerRadius);
                Debug.Log($"[EchoForge] Exit '{exit.Id}' at world({pos.X:F2}, {pos.Y:F2}) triggerRadius={exit.TriggerRadius}");
            }

            puzzleSystem.SetupLevel(plateEntities, doorEntities, switchEntities, exitEntity);
            Debug.Log($"[EchoForge] Level setup complete — plates:{plateEntities.Count} doors:{doorEntities.Count} switches:{switchEntities.Count} exit:{(exitEntity != null ? "yes" : "none")}");
        }

        private void ApplyCharacterControllerMovement(float deltaTime)
        {
            if (characterController == null || playerTransform == null) return;

            Vector3 currentPos = characterController.transform.position;
            Vector3 desiredPos = new Vector3(
                playerSystem.Player.Position.X,
                currentPos.y,
                playerSystem.Player.Position.Y);

            Vector3 moveDelta = new Vector3(
                desiredPos.x - currentPos.x,
                0f,
                desiredPos.z - currentPos.z);

            if (characterController.isGrounded)
                playerVelocity.y = -2f;
            else
                playerVelocity.y += Gravity * deltaTime;

            moveDelta.y = playerVelocity.y * deltaTime;

            characterController.Move(moveDelta);

            Vector3 corrected = characterController.transform.position;
            playerSystem.Player.Position = ToVec(corrected);
            playerYHeight = corrected.y;
        }

        private void SyncTransforms()
        {
            if (characterController == null && playerTransform != null)
            {
                Vector3 pos = FromVec(playerSystem.Player.Position);
                pos.y = playerYHeight;
                playerTransform.position = pos;
            }

            IReadOnlyList<Echo.EchoEntity> echoes = echoTimelineSystem.ActiveEchoes;
            for (int i = 0; i < echoObjects.Count && i < echoes.Count; i++)
            {
                if (echoObjects[i] != null)
                    echoObjects[i].transform.position = FromVec(echoes[i].Position);
            }
        }

        private void OnEchoSpawned(EchoSpawnedEvent e)
        {
            if (echoPrefab != null)
            {
                GameObject obj = Instantiate(echoPrefab, FromVec(e.Echo.Timeline.StartPosition), Quaternion.identity);
                echoObjects.Add(obj);
            }
            else
            {
                echoObjects.Add(null);
            }
        }

        private void OnLevelReset(LevelResetEvent e)
        {
            if (characterController != null && playerTransform != null)
            {
                characterController.enabled = false;
                playerTransform.position = new Vector3(
                    playerSystem.Player.Position.X,
                    playerYHeight,
                    playerSystem.Player.Position.Y);
                characterController.enabled = true;
            }

            for (int i = echoObjects.Count - 1; i >= 0; i--)
            {
                if (echoObjects[i] != null)
                    Destroy(echoObjects[i]);
            }
            echoObjects.Clear();

            if (doors != null)
            {
                foreach (DoorConfig cfg in doors)
                {
                    if (cfg?.Transform == null) continue;

                    if (doorColliders.TryGetValue(cfg.Id, out Collider col))
                        col.enabled = true;

                    foreach (DoorVisual v in doorVisuals)
                    {
                        if (v.Transform == cfg.Transform)
                        {
                            cfg.Transform.position = v.ClosedPosition;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < doorVisuals.Count; i++)
            {
                DoorVisual v = doorVisuals[i];
                v.IsOpen = false;
                doorVisuals[i] = v;
            }
        }

        private void OnDoorOpened(DoorOpenedEvent e)
        {
            if (doorColliders.TryGetValue(e.DoorId, out Collider col))
                col.enabled = !e.IsOpen;

            for (int i = 0; i < doorVisuals.Count; i++)
            {
                DoorVisual v = doorVisuals[i];
                if (v.Transform == null) continue;

                if (doors != null)
                {
                    foreach (DoorConfig cfg in doors)
                    {
                        if (cfg.Id == e.DoorId && cfg.OpenPositionTransform != null && v.Transform == cfg.Transform)
                        {
                            v.IsOpen = e.IsOpen;
                            doorVisuals[i] = v;
                        }
                    }
                }
            }
        }

        private void UpdateDoorVisuals(float deltaTime)
        {
            for (int i = 0; i < doorVisuals.Count; i++)
            {
                DoorVisual v = doorVisuals[i];
                if (v.Transform == null) continue;

                Vector3 target = v.IsOpen ? v.OpenPosition : v.ClosedPosition;
                v.Transform.position = Vector3.MoveTowards(
                    v.Transform.position, target, v.MoveSpeed * deltaTime);
            }
        }

        private void OnEchoDeleted(EchoDeletedEvent e)
        {
            if (echoObjects.Count > 0)
            {
                if (echoObjects[0] != null)
                    Destroy(echoObjects[0]);
                echoObjects.RemoveAt(0);
            }
        }

        private void OnDrawGizmos()
        {
            foreach ((Vector3 pos, float radius) in debugPlateZones)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
                Gizmos.DrawSphere(pos, radius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(pos, radius);
            }

            foreach ((Vector3 pos, float radius) in debugSwitchZones)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
                Gizmos.DrawSphere(pos, radius);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pos, radius);
            }

            if (debugExitZone.radius > 0f)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
                Gizmos.DrawSphere(debugExitZone.pos, debugExitZone.radius);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(debugExitZone.pos, debugExitZone.radius);
            }

            if (playerTransform != null && Application.isPlaying && playerSystem != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(playerTransform.position, 0.15f);
            }
        }

        private static VectorData ToVec(Vector3 v) => new VectorData(v.x, v.z);
        private Vector3 FromVec(VectorData v) => new Vector3(v.X, playerYHeight, v.Y);
    }
}
