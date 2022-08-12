using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Map;
using Licht.Impl.Orchestration;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseTile : EffectPoolable
{
    public LichtPhysicsObject PhysicsObject;
    public Vector2Int Position;
    public abstract bool Breakable { get; }
    public bool IsBroken => CurrentDurability <= 0;
    public int Durability;
    protected Player Player;
    protected Grid Grid;
    protected bool Dirty;
    public Dictionary<string, object> CustomProperties;

    private string _generatedBySeed;
    private List<ITileStateExtension> _extensions;
    private ITilePropGenerator[] _tileProps;
    private MapGenerator _mapGenerator;
    private Vector2Int[] _occupiedPropPositions = new Vector2Int[5];

    public string GeneratedBySeed
    {
        get => _generatedBySeed;
        set
        {
            _generatedBySeed = value;
            OnSeedChanged?.Invoke(value);
        }
    }

    public event Action<string> OnSeedChanged;
    public event Action OnBreak;
    public event Action OnRelease;

    protected abstract IEnumerable<IEnumerable<Action>> OnHitEffect(int damage, Vector2Int direction);

    protected virtual IEnumerable<IEnumerable<Action>> OnBreakEffect()
    {
        yield return TimeYields.WaitOneFrameX;
    }

    protected int CurrentDurability;

    protected IEnumerable<IEnumerable<Action>> Break()
    {
        OnBreak?.Invoke();
        yield return OnBreakEffect().AsCoroutine();
        ReleaseTile();
    }

    private MapManager _mapManager;

    protected override void OnAwake()
    {
        base.OnAwake();
        Player = SceneObject<Player>.Instance();
        Grid = SceneObject<Grid>.Instance();
        CustomProperties = new Dictionary<string, object>();
        _mapManager = SceneObject<MapManager>.Instance();
        _extensions = new List<ITileStateExtension>();
        _tileProps = GetComponentsInChildren<ITilePropGenerator>();
        _mapGenerator = SceneObject<MapGenerator>.Instance();
    }

    private void Update()
    {
        var player = MapManager.Get9X9Position(Grid.WorldToCell(Player.transform.position));
        var cell = MapManager.Get9X9Position((Vector3Int)Position);

        var reachedX = Math.Abs(player.x - cell.x) > 1;

        if (reachedX || Math.Abs(player.y - cell.y) > 1)
        {
            ReleaseTile();
        }
    }

    private void OnEnable()
    {
        CurrentDurability = Durability;
        PhysicsObject.AddCustomObject(this);
        _mapGenerator.OnPopulate += OnPopulate;
    }

    private void OnPopulate()
    {
        foreach (var tile in _tileProps)
        {
            tile.Populate(GeneratedBySeed);
        }

        if (CustomProperties != null)
        {
            foreach (var extension in _extensions)
            {
                extension.LoadState(CustomProperties);
            }
        }
        _mapGenerator.OnPopulate -= OnPopulate;
    }

    private void OnDisable()
    {
        OnBreak = null;
        PhysicsObject.RemoveCustomObject<BaseTile>();
        _mapGenerator.OnPopulate -= OnPopulate;
        if (_mapManager!= null && _mapManager.ActiveTiles!=null) _mapManager.ActiveTiles.Remove(Position);
    }

    public override void OnActivation()
    {
    }

    private void ReleaseTile()
    {
        for (var index = 0; index < _occupiedPropPositions.Length; index++)
        {
            _occupiedPropPositions[index] = Vector2Int.zero;
        }

        SaveTileChanges();
        _extensions.Clear();
        OnRelease?.Invoke();
        OnRelease = null;
        EndEffect();
    }

    public bool IsOccupiedByProp(Vector2Int pos)
    {
        return _occupiedPropPositions.Contains(pos);
    }

    public void AddTileExtension(ITileStateExtension extension, Vector2Int direction)
    {
        if (direction == Vector2Int.up) _occupiedPropPositions[0] = Vector2Int.up;
        if (direction == Vector2Int.right) _occupiedPropPositions[1] = Vector2Int.right;
        if (direction == Vector2Int.left) _occupiedPropPositions[2] = Vector2Int.left;
        if (direction == Vector2Int.down) _occupiedPropPositions[3] = Vector2Int.down;
        if (direction == Vector2Int.one) _occupiedPropPositions[4] = Vector2Int.one;
        _extensions.Add(extension);
    }

    public virtual TileChange GetTileChange()
    {
        return new TileChange
        {
            Destroyed = CurrentDurability <= 0,
            Durability = CurrentDurability,
            CustomProperties = CustomProperties.ToDictionary(k=>k.Key, v=>v.Value)
        };
    }

    public virtual void ApplyTileChanges()
    {
        _mapManager.ActiveTiles[Position] = this;

        if (_mapManager == null || _mapManager.TileChanges == null || !_mapManager.TileChanges.ContainsKey(Position))
        {
            CurrentDurability = Durability;
            ApplyDefaultTileSettings();
            return;
        }

        var tileChanges = _mapManager.TileChanges[Position];
        CurrentDurability = tileChanges.Durability;
        CustomProperties = tileChanges.CustomProperties;
        ApplyCustomTileChanges(tileChanges);
    }

    protected abstract void ApplyDefaultTileSettings();
    protected abstract void ApplyCustomTileChanges(TileChange tileChange);

    private void SaveTileChanges()
    {
        var tileChange = GetTileChange();
        var anyExtensionsDirty = false;
        foreach (var extension in _extensions)
        {
            if (!extension.Dirty) continue;
            extension.SaveState(tileChange.CustomProperties);
            anyExtensionsDirty = true;
        }
        if (!Dirty && !anyExtensionsDirty) return;
        _mapManager.SetTileChange(Position, tileChange);
    }

    public void Hit(int damage, Vector2Int direction)
    {
        if (Breakable)
        {
            Dirty = true;
            CurrentDurability -= damage;
            if (CurrentDurability <= 0)
            {
                DefaultMachinery.AddBasicMachine(Break());
                return;
            }
        }
        DefaultMachinery.AddBasicMachine(OnHitEffect(damage, direction));
    }
}
