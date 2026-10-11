using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using MEC;
using NorthwoodLib.Pools;
using ProjectMER.Events.Handlers;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Objects;
using Sakura.API.Utils;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Components;

[BlockComponent(ComponentType.Scp106SinkPaddle)]
public sealed class Scp106SinkPaddle : MonoBehaviour, IBlockComponent
{
    private const float TriggerDelay = 2f;
    private const float SinkDepth = 2.2f;
    private const float SinkDuration = 2f;
    private const float EmptyLifetime = 3f;
    
    private static readonly int EndSinkParam = Animator.StringToHash("End");
    private Transform _cacheTransform = null!;
    private readonly Dictionary<Player, float> _pendingPlayers = new();
    private readonly Dictionary<Player, float> _capturedPlayers = new();
    private readonly Dictionary<Player, Vector3> _sinkStartPositions = new();
    private float _emptySince;
    private bool _enabled;
    private Animator _animator;
    
    public static Scp106SinkPaddle? CreateSinkPaddle(Player victim)
    {
        if (!PlayerRaycast.TryGetFloorPosition(victim.Position, 2f, out var floorPosition))
        {
            return null;
        }
        
        var sinkPaddle = CreateSinkPaddle(floorPosition);
        if (sinkPaddle != null)
        {
            sinkPaddle.TrapPlayer(victim);
        }
        return sinkPaddle;
    }

    public static Scp106SinkPaddle? CreateSinkPaddle(Vector3 position)
    {
        if (!ObjectSpawner.TrySpawnSchematic("Scp106SinkPaddle", position, out var schematic))
        {
            return null;
        }

        var sinkPaddle = schematic.GetComponentInChildren<Scp106SinkPaddle>();
        return sinkPaddle;
    }
    
    private void Awake()
    {
        _cacheTransform = transform;
        _emptySince = Time.time;
    }

    private void Start()
    {
        _enabled = true;
        var primitive = GetComponent<PrimitiveObjectToy>();
        primitive._collider.enabled = true;
        primitive._collider.isTrigger = true;
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = Player.Get(other.gameObject);
        if (player is null)
            return;
        
        if (!_capturedPlayers.ContainsKey(player))
            _pendingPlayers.TryAdd(player, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = Player.Get(other.gameObject);
        if (player is not null)
            _pendingPlayers.Remove(player);
    }
    
    private void TrapPlayer(Player victim)
    {
        if (!_capturedPlayers.TryAdd(victim, 0f))
            return;

        _sinkStartPositions.Add(victim, victim.Position);
        
        victim.EnableEffect<Sinkhole>();
        victim.EnableEffect<Corroding>();
        victim.EnableEffect<Ensnared>();
        victim.IsNoclipEnabled = true;
    }

    private void EndSink(Player victim)
    {
        if (!_capturedPlayers.Remove(victim))
            return;

        _sinkStartPositions.Remove(victim);
        victim.DisableEffect<Sinkhole>();
        victim.DisableEffect<Corroding>();
        victim.DisableEffect<Ensnared>();
        victim.DisableEffect<Blindness>();
        victim.IsNoclipEnabled = false;
        victim.EnableEffect<PocketCorroding>();
    }

    private void Update()
    {
        if (!_enabled)
            return;
        ProcessSink();

        if (_capturedPlayers.Count > 0 || _pendingPlayers.Count > 0)
        {
            _emptySince = -1f;
            return;
        }

        if (_emptySince < 0f)
            _emptySince = Time.time;

        if (Time.time - _emptySince >= EmptyLifetime)
        {
            _enabled = false;
            _animator.SetTrigger(EndSinkParam);
            var schematic = GetComponentInParent<SchematicObject>();
            if (schematic != null)
            {
                Timing.CallDelayed(1f, schematic.Destroy);
            }
            else
            {
                Timing.CallDelayed(1f, () =>
                {
                    Destroy(gameObject);
                });
            }
        }
    }

    private void ProcessSink()
    {
        var list = ListPool<Player>.Shared.Rent(_capturedPlayers.Keys);
        
        foreach (var player in list)
        {
            var elapsed = _capturedPlayers[player] + Time.deltaTime;
            var targetPosition = _cacheTransform.position - Vector3.up * SinkDepth;
            player.Position = Vector3.Lerp(_sinkStartPositions[player], targetPosition, elapsed / SinkDuration);
            var blindnessIntensity = (byte)(Mathf.Clamp01(elapsed / SinkDuration) * 100f + 44);
            player.EnableEffect<Blindness>(blindnessIntensity);

            if (elapsed >= SinkDuration)
                EndSink(player);
            else
                _capturedPlayers[player] = elapsed;
        }
        
        list.Clear();
        list.AddRange(_pendingPlayers.Keys);
        var playersToTrap = ListPool<Player>.Shared.Rent();
        foreach (var player in list)
        {
            var elapsed = _pendingPlayers[player] + Time.deltaTime;
            if (elapsed < TriggerDelay)
            {
                _pendingPlayers[player] = elapsed;
                continue;
            }

            _pendingPlayers.Remove(player);
            playersToTrap.Add(player);
        }

        foreach (var player in playersToTrap)
        {
            TrapPlayer(player);
        }
        
        ListPool<Player>.Shared.Return(list);
        ListPool<Player>.Shared.Return(playersToTrap);
    }
    
    public ComponentData Compile()
    {
        return new ComponentData()
        {
            Type = ComponentType.Scp106SinkPaddle,
        };
    }

    public void Decompile(ComponentData componentData)
    {
        
    }
}
