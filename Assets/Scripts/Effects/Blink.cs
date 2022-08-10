using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class Blink : BaseUIObject
{
    public SpriteRenderer SpriteRenderer;
    public float FrequencyInMs;

    private bool _enabled;
    private void OnEnable()
    {
        _enabled = true;
        DefaultMachinery.AddBasicMachine(PerformBlink());
    }

    private void OnDisable()
    {
        _enabled = false;
    }

    private IEnumerable<IEnumerable<Action>> PerformBlink()
    {
        while (_enabled)
        {
            SpriteRenderer.enabled = !SpriteRenderer.enabled;
            yield return TimeYields.WaitMilliseconds(UITimer, FrequencyInMs);
        }
    }
}
