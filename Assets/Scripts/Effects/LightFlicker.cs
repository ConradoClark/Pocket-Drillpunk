using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Effects
{
    public class LightFlicker : BaseGameObject
    {
        public Light2D Light;
        public float MinIntensity;
        public float MaxIntensity;
        public float FrequencyInMs;

        private bool _enabled;

        private void OnEnable()
        {
            _enabled = true;
            DefaultMachinery.AddBasicMachine(Flicker());
        }

        private void OnDisable()
        {
            _enabled = false;
        }

        private IEnumerable<IEnumerable<Action>> Flicker()
        {
            while (_enabled)
            {
                Light.intensity = Random.Range(MinIntensity, MaxIntensity);
                yield return TimeYields.WaitMilliseconds(GameTimer, FrequencyInMs);
            }
        }
    }
}
