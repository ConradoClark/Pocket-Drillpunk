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
    public class HitEffect_BasicAnimation : HitEffect
    {
        [Serializable]
        public struct DamagePoints
        {
            public float TimeInSeconds;
            public float ImpactSize;
            public int Damage;
        }

        public Vector3 Position;
        public Animator Animator;
        public string State;
        public DamagePoints[] DamageImpacts;

        public override void OnActivation()
        {
            base.OnActivation();
            transform.position = Position;
            DefaultMachinery.AddBasicMachine(PerformEffect());
            DefaultMachinery.AddBasicMachine(HandleImpacts());
        }

        private IEnumerable<IEnumerable<Action>> PerformEffect()
        {
            yield return TimeYields.WaitOneFrameX;
            if (Target == null) yield break;
            yield return TimeYields.WaitSeconds(UITimer, 1f);
            Animator.Play(State);
        }

        private IEnumerable<IEnumerable<Action>> HandleImpacts()
        {
            yield return TimeYields.WaitOneFrameX;
            if (Target == null) yield break;

            var damageSum = 0;
            for (var index = 0; index < DamageImpacts.Length; index++)
            {
                var impact = DamageImpacts[index];
                yield return TimeYields.WaitSeconds(UITimer, impact.TimeInSeconds);
                
                Target.Hit(index == DamageImpacts.Length-1 ? Math.Clamp(TotalDamage - damageSum, 0, int.MaxValue) : impact.Damage);
                damageSum += impact.Damage;

                yield return MakeImpact(impact.ImpactSize).AsCoroutine();
            }
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
