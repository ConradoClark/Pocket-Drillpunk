using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class EnemyBattler : BaseBattler
    {
        public BattleElement Element;
        public int Experience;
        public int HP;
        public Color HitColor;
        public SpriteRenderer SpriteRenderer;
        public ScriptPrefab SpawnEffect;
        public ScriptPrefab DeathEffect;
        public Sprite NameSprite;

        public EnemySkill EnemyAttack; // Temporary
        public EnemyAttackSelector AttackSelector;

        private BattleSequence _battleSequence;
        private static readonly Color TransparentColor = new Color(0, 0, 0, 0);
        private IPoolableComponent _currentDeathEffect;

        public EnemySkill SelectAction()
        {
            return AttackSelector == null ? EnemyAttack : AttackSelector.SelectAction();
        }

        public override void Shield(int shield)
        {
            CurrentShield = shield;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _battleSequence = SceneObject<BattleSequence>.Instance(true);
        }

        public override void OnActivation()
        {
            base.OnActivation();
            DefaultMachinery.AddBasicMachine(Spawn());
        }

        private void OnDisable()
        {
            if (_currentDeathEffect is { IsActive: true })
            {
                DeathEffect.Pool.Release(_currentDeathEffect);
            }
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

        public override event Action<int> OnHit;

        public override void Hit(int damage)
        {
            OnHit?.Invoke(damage);
            DefaultMachinery.AddBasicMachine(OnHitEffect(damage));
        }

        private IEnumerable<IEnumerable<Action>> OnHitEffect(int damage)
        {
            _battleSequence.DamageNumberRenderer.transform.position = _battleSequence.DamageNumberOriginalPosition;
            _battleSequence.DamageNumberRenderer.transform.localScale = new Vector3(1, 0.1f, 1);
            _battleSequence.DamageNumberRenderer.enabled = true;
            _battleSequence.DamageNumberRenderer.Number = damage;

            SpriteRenderer.material.SetColor("_Colorize", HitColor);

            var blink = SpriteRenderer.GetAccessor()
                .Material("_Colorize")
                .AsColor()
                .ToColor(TransparentColor)
                .Over(0.5f)
                .WithStep(0.1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(GameTimer)
                .Build();

            var damageNumberAnim = _battleSequence.DamageNumberRenderer
                .transform.GetAccessor()
                .Position
                .Y
                .Increase(0.02f)
                .Over(0.5f)
                .UsingTimer(GameTimer)
                .Build();

            var damageNumberAnimScale1 = _battleSequence.DamageNumberRenderer
                .transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(2)
                .Over(0.3f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(UITimer)
                .Build();

            var damageNumberAnimScale2 = _battleSequence.DamageNumberRenderer
                .transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(1)
                .Over(0.2f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(UITimer)
                .Build();

            yield return blink.Combine(damageNumberAnim).Combine(damageNumberAnimScale1.Then(damageNumberAnimScale2));

            var damageNumberScale = _battleSequence.DamageNumberRenderer
                .transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(0.01f)
                .Over(0.2f)
                .UsingTimer(GameTimer)
                .Build();

            yield return damageNumberScale;

            _battleSequence.DamageNumberRenderer.enabled = false;
        }

        public override void Die()
        {
            base.Die();
            DefaultMachinery.AddBasicMachine(ShowDeathEffect());
        }

        private IEnumerable<IEnumerable<Action>> ShowDeathEffect()
        {
            if (DeathEffect.Pool.TryGetFromPool(out _currentDeathEffect))
            {
                _currentDeathEffect.Component.transform.position = transform.position;
                _currentDeathEffect.Component.transform.SetParent(transform);
            }

            yield return transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(0.01f)
                .Over(2f)
                .UsingTimer(GameTimer)
                .Build();
        }
    }
}
