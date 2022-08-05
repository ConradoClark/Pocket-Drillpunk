using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;

namespace Assets.Scripts.Inventory
{
    public class CounterCollectable : Collectable
    {
        public ScriptPrefab CollectableEffect;
        public ScriptPrefab UICollectableEffect;
        public Counter Counter;

        private ITimer _uiTimer;
        protected override void OnAwake()
        {
            base.OnAwake();
            _uiTimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
        }

        protected override IEnumerable<IEnumerable<Action>> CollectEffect()
        {
            if (CollectableEffect.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }

            if (UICollectableEffect.Pool.TryGetFromPool(out var uiEffect))
            {
                uiEffect.Component.transform.SetParent(transform);
                DefaultMachinery.AddBasicMachine(UIEffect());
            }
            else
            {
                Counter.Count++;
            }

            yield break;
        }

        private IEnumerable<IEnumerable<Action>> UIEffect()
        {
            yield return TimeYields.WaitSeconds(_uiTimer, 0.8f);
            Counter.Count++;
        }
    }
}
