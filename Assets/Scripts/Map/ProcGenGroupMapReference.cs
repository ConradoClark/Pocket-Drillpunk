using System;
using System.Linq;
using Licht.Impl.Generation;
using Licht.Unity.Objects;
using UnityEngine;

public class ProcGenGroupMapReference : ProcGenMapReference
{
    public int DefaultWeight;
    public Vector2Int Size;
    public override Vector2Int RefSize => Size;
    public ScriptIdentifier[] References;

    protected override void OnAwake()
    {
        base.OnAwake();
        Weight = DefaultWeight;
    }

    public override void Populate(string seed, Vector2Int position)
    {
        PopulatedPositions.Add(position);

        var rules = MapGenerator.GlobalRules;
        ScriptIdentifier forcedTile = null;
        var forceInOffsets = false;
        Vector2Int[] forcedOffsets = Array.Empty<Vector2Int>();

        foreach (var @ref in References)
        {
            foreach (var rule in rules)
            {
                if (rule.HasMinXLimit && position.x < rule.MinXLimit) continue;
                if (rule.HasMaxXLimit && position.x > rule.MaxXLimit) continue;

                if (rule.HasMinDepthLimit && position.y < rule.MinDepthLimit) continue;
                if (rule.HasMaxDepthLimit && position.y > rule.MaxDepthLimit) continue;
                if (rule.ForceTile != null)
                {
                    forcedTile = rule.ForceTile;
                    forceInOffsets = rule.ForceInOffsets;
                    forcedOffsets = rule.ForcedOffsets;
                    break;
                }
                foreach (var weightRule in rule.WeightRules)
                {
                    if (weightRule.TileIdentifier == @ref)
                    {
                        GameTilemap.TileDefinitionsDictionary[weightRule.TileIdentifier].Reference.Weight = weightRule.Weight;
                    }
                }
            }
        }

        for (var i = 0; i < Size.x; i++)
        {
            for (var j = 0; j < Size.y; j++)
            {
                var useForcedTile = true;
                if (forceInOffsets)
                {
                    useForcedTile = forcedOffsets.Contains(new Vector2Int(j, i));
                }

                var newSeed = $"{seed}_group_X{position.x + i}_Y{position.y + j}";
                Seed = newSeed.GetHashCode();
                var reference = useForcedTile && forcedTile != null
                    ? GameTilemap.TileDefinitionsDictionary[forcedTile].Reference
                    : GetReference();

                if (reference == null)
                {
                    throw new Exception("ProcGenMapReference 'Reference' is null for " + gameObject.name);
                }

                if (reference.RefSize.x <= 0 || reference.RefSize.y <= 0)
                {
                    throw new Exception($"ProcGenMapReference 'Reference' cannot have zero/negative size ({gameObject.name})");
                }

                if (i % reference.RefSize.x != 0 || j % reference.RefSize.y != 0) continue;

                reference.Populate(newSeed, position+ new Vector2Int(i,j));
            }
        }
    }

    private ProcGenMapReference GetReference()
    {
        ProcGenMapReference reference;

        var references = References.Select(@ref => GameTilemap.TileDefinitionsDictionary[@ref].Reference).ToArray();
        if (references.Length == 1)
        {
            reference = references[0];
        }
        else
        {
            var dice = new WeightedDice<ProcGenMapReference>(references, this);
            reference = dice.Generate();
        }

        if (reference == null)
        {
            throw new Exception("ProcGenMapReference 'Reference' is null for " + gameObject.name);
        }

        if (reference.RefSize.x <= 0 || reference.RefSize.y <= 0)
        {
            throw new Exception($"ProcGenMapReference 'Reference' cannot have zero/negative size ({gameObject.name})");
        }

        return reference;
    }
}
