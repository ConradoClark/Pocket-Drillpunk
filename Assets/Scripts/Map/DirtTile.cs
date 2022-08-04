using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;
using Random = UnityEngine.Random;

public class DirtTile : BaseTile
{
    public SpriteRenderer SpriteRenderer;
    private Vector2 _crackDirection;


    protected override void OnAwake()
    {
        base.OnAwake();
        SpriteRenderer.material.SetFloat("_CrackScale", Random.Range(10f, 22f));
    }

    public override void OnActivation()
    {
        base.OnActivation();
        _crackDirection = Vector2.zero;
    }

    public override bool Breakable => true;

    protected override IEnumerable<IEnumerable<Action>> OnHitEffect(int damage, Vector2Int direction)
    {

        if (_crackDirection == Vector2.zero)
        {
            _crackDirection = direction;
            SpriteRenderer.material.SetVector("_Direction", new Vector2(direction.x * 0.5f, direction.y * 0.5f));
        }
        SpriteRenderer.material.SetFloat("_CrackSize", 3f * ((Durability - CurrentDurability) / (float)Durability));
        yield return TimeYields.WaitOneFrameX;
    }

    protected override IEnumerable<IEnumerable<Action>> OnBreakEffect()
    {
        yield return TimeYields.WaitOneFrameX;
    }
}
