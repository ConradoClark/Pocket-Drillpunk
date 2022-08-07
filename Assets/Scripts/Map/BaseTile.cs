using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;

public abstract class BaseTile : EffectPoolable
{
    public LichtPhysicsObject PhysicsObject;
    public Vector2Int Position;
    public abstract bool Breakable { get; }
    public int Durability;
    protected Player Player;
    protected Grid Grid;
    protected bool Dirty;
    public Dictionary<string, object> CustomProperties;

    protected abstract IEnumerable<IEnumerable<Action>> OnHitEffect(int damage, Vector2Int direction);

    protected virtual IEnumerable<IEnumerable<Action>> OnBreakEffect()
    {
        yield return TimeYields.WaitOneFrameX;
    }

    protected int CurrentDurability;

    protected IEnumerable<IEnumerable<Action>> Break()
    {
        yield return OnBreakEffect().AsCoroutine();
        SaveTileChanges();
        EndEffect();
    }

    private MapManager _mapManager;

    protected override void OnAwake()
    {
        base.OnAwake();
        Player = SceneObject<Player>.Instance();
        Grid = SceneObject<Grid>.Instance();
        CustomProperties = new Dictionary<string, object>();
        _mapManager = SceneObject<MapManager>.Instance();
    }

    private void Update()
    {
        var player = MapManager.Get9X9Position(Grid.WorldToCell(Player.transform.position));
        var cell = MapManager.Get9X9Position((Vector3Int)Position);

        var reachedX = Math.Abs(player.x - cell.x) > 1;

        if (reachedX || Math.Abs(player.y - cell.y) > 1)
        {
            SaveTileChanges();
            EndEffect();
        }
    }

    private void OnEnable()
    {
        CurrentDurability = Durability;
        PhysicsObject.AddCustomObject(this);
    }

    private void OnDisable()
    {
        PhysicsObject.RemoveCustomObject<BaseTile>();
    }

    public override void OnActivation()
    {
        ApplyTileChanges();
    }

    public virtual TileChange GetTileChange()
    {
        return new TileChange
        {
            Destroyed = CurrentDurability <= 0,
            Durability = CurrentDurability
        };
    }

    public virtual void ApplyTileChanges()
    {
        if (_mapManager == null || _mapManager.TileChanges == null || !_mapManager.TileChanges.ContainsKey(Position)) return;
        var tileChanges = _mapManager.TileChanges[Position];
        CurrentDurability = tileChanges.Durability;
        CustomProperties = tileChanges.CustomProperties;
        ApplyCustomTileChanges(tileChanges);
    }

    protected abstract void ApplyCustomTileChanges(TileChange tileChange);

    private void SaveTileChanges()
    {
        if (!Dirty) return;
        var tileChange = GetTileChange();
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
