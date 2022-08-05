using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

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
    }

    public RotationDefinition[] Definitions;

    private void OnEnable()
    {
        var rotation = Definitions.Aggregate(Quaternion.identity, (current, def) => 
            current * Quaternion.AngleAxis(Random.Range(def.MinValue, def.MaxValue), GetAxis(def.Axis)));

        transform.rotation = rotation;
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
