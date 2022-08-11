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
    public class HeartTile : DirtTile
    {
        private PlayerStats _stats;
        protected override void OnAwake()
        {
            base.OnAwake();
            _stats = SceneObject<PlayerStats>.Instance();
        }

        protected override IEnumerable<IEnumerable<Action>> OnBreakEffect()
        {
            if (ExplosionEffect.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }

            _stats.IncreaseHP(1);
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
