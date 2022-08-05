using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Inventory;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class MoveToCounterEffect : EffectPoolable
{
    public Counter Counter;
    private Camera _uiCamera;
    private ITimer _uiTimer;
    protected override void OnAwake()
    {
        base.OnAwake();
        _uiCamera = SceneObject<UICamera>.Instance().Camera;
        _uiTimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
    }

    private IEnumerable<IEnumerable<Action>> Move()
    {
        yield return TimeYields.WaitOneFrameX;

        transform.position = _uiCamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.parent.position));
        transform.SetParent(null);

        if (UINumberUpdater.Counters.ContainsKey(Counter))
        {
            var counter = UINumberUpdater.Counters[Counter];

            var scale = transform.GetAccessor()
                .UniformScale()
                .SetTarget(0.4f)
                .Over(0.8f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(_uiTimer)
                .Build();

            yield return PositionAnim(counter).AsCoroutine().Combine(scale);
        }

        EndEffect();
    }

    private IEnumerable<IEnumerable<Action>> PositionAnim(UINumberUpdater counter)
    {

        yield return transform.GetAccessor()
            .Position
            .ToPosition(transform.position - counter.transform.position * 0.2f)
            .Over(0.2f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_uiTimer)
            .Build();

        yield return transform.GetAccessor()
            .Position
            .ToPosition(counter.transform.position)
            .Over(0.6f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(_uiTimer)
            .Build();
    }

    public override void OnActivation()
    {
        transform.localScale = Vector3.one;
        DefaultMachinery.AddBasicMachine(Move());
    }
}
