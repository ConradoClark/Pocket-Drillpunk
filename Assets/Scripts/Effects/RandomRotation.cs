using System;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(9999)]
public class RandomRotation : MonoBehaviour
{
    [Serializable]
    public struct RotationDefinition
    {
        [Range(-359f, 359f)]
        public float MinValue;
        [Range(-359f, 359f)]
        public float MaxValue;
        public TransformExtensions.Axis Axis;
        public float Step;
    }

    public BaseTile Tile;
    public RotationDefinition[] Definitions;

    private void OnEnable()
    {
        if (Tile == null)
        {
            SetRotation();
            return;
        }

        Tile.OnSeedChanged += Tile_OnSeedChanged;
    }

    private void SetRotation()
    {
        var rotation = Definitions.Aggregate(Quaternion.identity, (current, def) =>
            current * Quaternion.AngleAxis(def.Step > 0
                ? EasingYields.GetStep(GetRandomRange(def.MinValue, def.MaxValue), def.Step)
                : GetRandomRange(def.MinValue, def.MaxValue), GetAxis(def.Axis)));

        transform.rotation = rotation;
    }

    private void OnDisable()
    {
        if (Tile!=null) Tile.OnSeedChanged -= Tile_OnSeedChanged;
    }

    private void Tile_OnSeedChanged(string obj)
    {
        SetRotation();
    }

    private float GetRandomRange(float minValue, float maxValue)
    {
        if (Tile == null)
        {
            return Random.Range(minValue, maxValue);
        }

        return (float) new System.Random(Tile.GeneratedBySeed.GetHashCode()).NextDouble() * (maxValue - minValue);
    }

    private static Vector3 GetAxis(TransformExtensions.Axis axis)
    {
        return axis switch
        {
            TransformExtensions.Axis.X => Vector3.right,
            TransformExtensions.Axis.Y => Vector3.up,
            TransformExtensions.Axis.Z => Vector3.forward,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };
    }
}
