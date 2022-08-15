using System;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class DrillBattler : BaseBattler
    {
        public Color HitColor;
        public SpriteRenderer SpriteRenderer;
        public Vector3 BattlerPosition;

        private Player _player;
        private Camera _gameCamera;
        private Camera _uiCamera;
        private ITimer _gameTimer;
        private BattleSequence _battleSequence;
        private static readonly Color TransparentColor = new Color(0, 0, 0, 0);

        public override event Action<int> OnHit;

        public override void Shield(int shield)
        {
            CurrentShield = shield;
        }
        protected override void OnAwake()
        {
            base.OnAwake();
            _player = SceneObject<Player>.Instance(true);
            _gameCamera = SceneObject<GameCamera>.Instance().Camera;
            _uiCamera = SceneObject<UICamera>.Instance().Camera;
            _gameTimer = SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
            _battleSequence = SceneObject<BattleSequence>.Instance(true);
        }

        public IEnumerable<IEnumerable<Action>> ActivateAndMoveToPosition()
        {
            gameObject.SetActive(true);

            var playerViewportPos = _gameCamera.WorldToViewportPoint(_player.transform.position);
            var uiPos = _uiCamera.ViewportToWorldPoint(playerViewportPos);

            transform.position = new Vector3(uiPos.x, uiPos.y, 0);
            yield return transform.GetAccessor()
                .Position
                .ToPosition(BattlerPosition)
                .Over(4f)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(UITimer)
                .Build();

        }

        public IEnumerable<IEnumerable<Action>> MoveBackAndDeactivate()
        {
            Animator.Play("DrillBattler_Intro");

            var playerViewportPos = _gameCamera.WorldToViewportPoint(_player.transform.position);
            var uiPos = _uiCamera.ViewportToWorldPoint(playerViewportPos);

            yield return transform.GetAccessor()
                .Position
                .ToPosition(uiPos)
                .Over(4f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            gameObject.SetActive(false);
        }

        private void Update()
        {
            Animator.speed = (float)_gameTimer.Multiplier;
        }

        public override void Hit(int damage)
        {
            OnHit?.Invoke(damage);
            DefaultMachinery.AddBasicMachine(OnHitEffect(damage));
        }

        private IEnumerable<IEnumerable<Action>> OnHitEffect(int damage)
        {
            _battleSequence.ReceivedDamageNumberRenderer.transform.position = _battleSequence.ReceivedDamageNumberOriginalPosition;
            _battleSequence.ReceivedDamageNumberRenderer.transform.localScale = new Vector3(1, 0.1f, 1);
            _battleSequence.ReceivedDamageNumberRenderer.enabled = true;
            _battleSequence.ReceivedDamageNumberRenderer.Number = damage;

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

            var damageNumberAnim = _battleSequence.ReceivedDamageNumberRenderer
                .transform.GetAccessor()
                .Position
                .Y
                .Increase(0.02f)
                .Over(0.5f)
                .UsingTimer(GameTimer)
                .Build();

            var damageNumberAnimScale1 = _battleSequence.ReceivedDamageNumberRenderer
                .transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(2)
                .Over(0.3f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(UITimer)
                .Build();

            var damageNumberAnimScale2 = _battleSequence.ReceivedDamageNumberRenderer
                .transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(1)
                .Over(0.2f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(UITimer)
                .Build();

            yield return blink.Combine(damageNumberAnim).Combine(damageNumberAnimScale1.Then(damageNumberAnimScale2));

            var damageNumberScale = _battleSequence.ReceivedDamageNumberRenderer
                .transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(0.01f)
                .Over(0.2f)
                .UsingTimer(GameTimer)
                .Build();

            yield return damageNumberScale;

            _battleSequence.ReceivedDamageNumberRenderer.enabled = false;
        }

    }
}
