using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class EnemyBattler : EffectPoolable
    {
        public SpriteRenderer SpriteRenderer;
        public ScriptPrefab SpawnEffect;

        public override void OnActivation()
        {
            DefaultMachinery.AddBasicMachine(Spawn());
        }

        private IEnumerable<IEnumerable<Action>> Spawn()
        {
            SpriteRenderer.color = new Color(0, 1, 0, 0);
            yield return TimeYields.WaitOneFrameX;

            if (SpawnEffect.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }

            yield return SpriteRenderer.GetAccessor()
                .Color
                .ToColor(Color.white)
                .Over(2f)
                .WithStep(0.1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();
        }
    }
}
