using System;
using System.Collections.Generic;
using Assets.Scripts.Inventory;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class BattleSequence : BaseUIObject
    {
        public UITextRenderer ActionNameCaption;
        public UINumberRenderer DamageNumberRenderer;
        public Vector3 DamageNumberOriginalPosition;

        public Counter EnemyHPCounter;

        private bool _battleOver;
        private UIActionSelectorBar _actionSelectorBar;
        private DrillBattler _drillBattler;
        private HitEffectPoolManager _hitPoolManager;
        private EnemyBattler _enemy;

        protected override void OnAwake()
        {
            base.OnAwake();
            _actionSelectorBar = SceneObject<UIActionSelectorBar>.Instance(true);
            _drillBattler = SceneObject<DrillBattler>.Instance(this);
            _hitPoolManager = SceneObject<HitEffectPoolManager>.Instance(true);
        }

        public void StartBattle(EnemyBattler enemy)
        {
            _enemy = enemy;
            EnemyHPCounter.Count = _enemy.HP;
            _enemy.OnHit += Enemy_OnHit;

            _battleOver = false;
            DefaultMachinery.AddBasicMachine(Battle());
        }

        private void Enemy_OnHit(int damage)
        {
            EnemyHPCounter.Count -= damage;
        }

        private IEnumerable<IEnumerable<Action>> Battle()
        {
            while (!_battleOver)
            {
                yield return DecisionPhase().AsCoroutine();
                yield return ActionPhase().AsCoroutine();
                yield return TimeYields.WaitOneFrameX;
            }
        }

        private IEnumerable<IEnumerable<Action>> DecisionPhase()
        {
            yield return _actionSelectorBar.Show().AsCoroutine();
            yield return TimeYields.WaitOneFrameX;
        }

        private IEnumerable<IEnumerable<Action>> ActionPhase()
        {
            ActionNameCaption.Text = _actionSelectorBar.SelectedAction.Name;
            ActionNameCaption.DefaultMaterial = _actionSelectorBar.GetActionMaterial();

            yield return PlayAction().AsCoroutine();

            ActionNameCaption.Text = "";

            yield return TimeYields.WaitOneFrameX;
        }

        private IEnumerable<IEnumerable<Action>> PlayAction()
        {
            if (!string.IsNullOrWhiteSpace(_actionSelectorBar.SelectedAction.Animation)) _drillBattler.PlayAnim(_actionSelectorBar.SelectedAction.Animation);

            HitEffect effect = null;
            if (_actionSelectorBar.SelectedAction.SkillEffect != null)
            {
                yield return TimeYields.WaitSeconds(UITimer, _actionSelectorBar.SelectedAction.EffectDelayInSeconds);
                var effectPool = _hitPoolManager.GetEffect(_actionSelectorBar.SelectedAction.SkillEffect);
                if (effectPool.TryGetFromPool(out effect))
                {
                    effect.Enemy = _enemy;
                }
            }

            yield return TimeYields.WaitSeconds(UITimer, _actionSelectorBar.SelectedAction.DurationInSeconds);

            if (effect != null)
            {
                effect.EndEffect();
            }
        }
    }
}
