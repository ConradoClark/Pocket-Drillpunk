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


    protected abstract IEnumerable<IEnumerable<Action>> OnHitEffect(int damage, Vector2Int direction);

    protected virtual IEnumerable<IEnumerable<Action>> OnBreakEffect()
    {
        yield return TimeYields.WaitOneFrameX;
    }

    protected int CurrentDurability;

    protected IEnumerable<IEnumerable<Action>> Break()
    {
        yield return OnBreakEffect().AsCoroutine();
        EndEffect();
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        Player = SceneObject<Player>.Instance();
        Grid = SceneObject<Grid>.Instance();
    }

    private void Update()
    {
        var player = MapManager.Get9X9Position(Grid.WorldToCell(Player.transform.position));
        var cell = MapManager.Get9X9Position((Vector3Int)Position);

        var reachedX = Math.Abs(player.x - cell.x) > 1;

        if (reachedX || Math.Abs(player.y - cell.y) > 1)
        {
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
    }

    public void Hit(int damage, Vector2Int direction)
    {
        if (Breakable)
        {
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
