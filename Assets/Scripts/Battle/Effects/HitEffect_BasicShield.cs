using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using UnityEngine;

namespace Assets.Scripts.Battle.Effects
{
    public class HitEffect_BasicShield : HitEffect
    {

        public Vector3 Position;
        public Animator Animator;
        public string State;
        public float TimeInSeconds;

        public override void OnActivation()
        {
            base.OnActivation();
            transform.position = Position;
            DefaultMachinery.AddBasicMachine(PerformEffect());
        }

        private IEnumerable<IEnumerable<Action>> PerformEffect()
        {
            yield return TimeYields.WaitOneFrameX;
            if (Target == null) yield break;
            yield return TimeYields.WaitSeconds(UITimer, 1f);
            Animator.Play(State);

            yield return TimeYields.WaitSeconds(UITimer, TimeInSeconds);
            DefaultMachinery.AddBasicMachine(MakeImpact(1));
            Target.Shield(TotalDamage);
        }

        private IEnumerable<IEnumerable<Action>> MakeImpact(float size)
        {
            GameTimer.Multiplier = 0;
            yield return new LerpBuilder(f => GameTimer.Multiplier = f, () => (float)GameTimer.Multiplier)
                .SetTarget(1)
                .Over(size * 0.25f)
                .UsingTimer(UITimer)
                .Easing(EasingYields.EasingFunction.ExponentialEaseIn)
                .Build();

            GameTimer.Multiplier = 1;
        }

        private void Update()
        {
            Animator.speed = (float) GameTimer.Multiplier;
        }
    }
}
