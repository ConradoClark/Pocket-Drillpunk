using System;
using System.Collections.Generic;
using Assets.Scripts.Inventory;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class DirtTile : BaseTile
{
    public ScriptPrefab ExplosionEffect;
    public ScriptPrefab ScrapPrefab;
    public SpriteRenderer SpriteRenderer;
    private Vector2 _crackDirection;
    protected override void OnAwake()
    {
        base.OnAwake();
        SpriteRenderer.material.SetFloat("_CrackScale", Random.Range(10f, 22f));
    }

    public override void OnActivation()
    {
        _crackDirection = CustomProperties.ContainsKey("CrackDirection")
            ? (Vector2)CustomProperties["CrackDirection"] : Vector2.zero;
        base.OnActivation();
    }

    public override TileChange GetTileChange()
    {
        var @base = base.GetTileChange();
        return new TileChange
        {
            Destroyed = @base.Destroyed,
            Durability = @base.Durability,
            CustomProperties = new Dictionary<string, object>
            {
                {"CrackDirection", _crackDirection}
            }
        };
    }

    protected override void ApplyCustomTileChanges(TileChange tileChange)
    {
        SpriteRenderer.material.SetVector("_Direction", new Vector2(_crackDirection.x * 0.5f, _crackDirection.y * 0.5f));
        SpriteRenderer.material.SetFloat("_CrackSize", 3f * ((Durability - CurrentDurability) / (float)Durability));
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
        if (ExplosionEffect.Pool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = transform.position;
        }

        if (ScrapPrefab.Pool.TryGetManyFromPool(Random.Range(1, 4), out var scraps))
        {
            foreach (var scrap in scraps)
            {
                scrap.Component.transform.position = transform.position;
            }
        }

        yield return TimeYields.WaitOneFrameX;
    }
}
