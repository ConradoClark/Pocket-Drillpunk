using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class CinemachineBrainUpdater : MonoBehaviour
{
    public ScriptBasicMachinery Updater;
    private CinemachineBrain _cinemachineBrain;
    private bool _enabled;
    private void Awake()
    {
        _cinemachineBrain = GetComponent<CinemachineBrain>();
    }

    private void OnEnable()
    {
        _enabled = true;
        Updater.Machinery.AddBasicMachine(UpdateCinemachineBrain());
    }

    private void OnDisable()
    {
        _enabled = false;
    }

    private IEnumerable<IEnumerable<Action>> UpdateCinemachineBrain()
    {
        while (_enabled)
        {
            _cinemachineBrain.ManualUpdate();
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
