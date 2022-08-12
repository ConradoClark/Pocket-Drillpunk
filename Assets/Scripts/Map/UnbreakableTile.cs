using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class UnbreakableTile : BaseTile
{
    public override TileChange GetTileChange()
    {
        var @base = base.GetTileChange();
        return new TileChange
        {
            Destroyed = @base.Destroyed,
            Durability = @base.Durability,
        };
    }

    protected override void ApplyCustomTileChanges(TileChange tileChange)
    {
    }

    protected override void ApplyDefaultTileSettings()
    {
    }

    public override bool Breakable => false;

    protected override IEnumerable<IEnumerable<Action>> OnHitEffect(int damage, Vector2Int direction)
    {
        yield return TimeYields.WaitOneFrameX;
    }

    protected override IEnumerable<IEnumerable<Action>> OnBreakEffect()
    {
        yield return TimeYields.WaitOneFrameX;
    }
}
