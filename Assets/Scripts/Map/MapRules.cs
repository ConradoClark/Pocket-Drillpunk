using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

[Serializable]
public class MapRules : MonoBehaviour
{
    public bool HasMinXLimit;
    public int MinXLimit;
    public bool HasMaxXLimit;
    public int MaxXLimit;

    public bool HasMinDepthLimit;
    public int MinDepthLimit;
    public bool HasMaxDepthLimit;
    public int MaxDepthLimit;

    public ScriptIdentifier ForceTile;
    public bool ForceInOffsets;
    public Vector2Int[] ForcedOffsets;
    public WeightRule[] WeightRules;

    [Serializable]
    public struct WeightRule
    {
        public ScriptIdentifier TileIdentifier;
        public float Weight;
    }
}


