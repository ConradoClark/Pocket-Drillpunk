using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;

namespace Assets.Scripts.Battle
{
    public class HitEffect : EffectPoolable
    {
        public EnemyBattler Enemy;
        protected ITimer UITimer;
        protected override void OnAwake()
        {
            base.OnAwake();
            UITimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
        }

        public override void OnActivation()
        {
            
        }
    }
}
