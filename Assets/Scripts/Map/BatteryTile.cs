using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Drill;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

namespace Assets.Scripts.Map
{
    public class BatteryTile : DirtTile
    {
        public int RefillAmount;
        private JetpackBattery _battery;
        protected override void OnAwake()
        {
            base.OnAwake();
            _battery = SceneObject<JetpackBattery>.Instance();
        }

        private IEnumerable<IEnumerable<Action>> Refill()
        {
            for (var i = 0; i < 30; i++)
            {
                _battery.Refill(RefillAmount);
                yield return TimeYields.WaitMilliseconds(GameTimer, 10);
            }
        }

        protected override IEnumerable<IEnumerable<Action>> OnBreakEffect()
        {
            if (ExplosionEffect.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }

            DefaultMachinery.AddBasicMachine(Refill());
            
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
