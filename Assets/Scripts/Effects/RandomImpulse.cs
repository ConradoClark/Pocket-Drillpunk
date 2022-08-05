using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomImpulse : BaseGameObject
{
    public float Speed;
    public LichtPhysicsObject PhysicsObject;

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleImpulse());
    }

    private IEnumerable<IEnumerable<Action>> HandleImpulse()
    {
        yield return PhysicsObject.GetSpeedAccessor(Random.insideUnitCircle * Speed)
            .ToSpeed(Vector2.zero)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();
    }
}
