using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;

public abstract class BaseTile : EffectPoolable
{
    public LichtPhysicsObject PhysicsObject;
    public Vector2Int Position;
    public abstract bool Breakable { get; }
    public int Durability;

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

    private void OnEnable()
    {
        CurrentDurability = Durability;
        PhysicsObject.AddCustomObject(this);

        // REMOVE THIS AFTER POOLING
        OnActivation();
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
