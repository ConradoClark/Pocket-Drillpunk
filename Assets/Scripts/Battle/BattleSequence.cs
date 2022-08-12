using System;
using System.Collections.Generic;
using Assets.Scripts.Drill;
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
        public UITextRenderer EnemyActionNameCaption;

        public UINumberRenderer DamageNumberRenderer;
        public Vector3 DamageNumberOriginalPosition;

        public UINumberRenderer ReceivedDamageNumberRenderer;
        public Vector3 ReceivedDamageNumberOriginalPosition;

        public Counter PlayerHPCounter;
        public Counter EnemyHPCounter;

        private bool _battleOver;
        private UIActionSelectorBar _actionSelectorBar;
        private UIEnemyActionBar _enemyActionBar;
        private DrillBattler _drillBattler;
        private HitEffectPoolManager _hitPoolManager;
        public EnemyBattler Enemy { get; private set; }
        private PlayerStats _playerStats;

        private BattleIntro _battleIntro;

        protected override void OnAwake()
        {
            base.OnAwake();
            _actionSelectorBar = SceneObject<UIActionSelectorBar>.Instance(true);
            _enemyActionBar = SceneObject<UIEnemyActionBar>.Instance(true);
            _drillBattler = SceneObject<DrillBattler>.Instance(this);
            _hitPoolManager = SceneObject<HitEffectPoolManager>.Instance(true);
            _battleIntro = SceneObject<BattleIntro>.Instance(true);
            _playerStats = SceneObject<PlayerStats>.Instance(true);
        }

        public void StartBattle(EnemyBattler enemy)
        {
            Enemy = enemy;
            EnemyHPCounter.Count = Enemy.HP;
            Enemy.OnHit += Enemy_OnHit;
            _drillBattler.OnHit += Player_OnHit;

            _battleOver = false;
            DefaultMachinery.AddBasicMachine(Battle());
        }

        private void Player_OnHit(int damage)
        {
            PlayerHPCounter.Count -= damage;
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

                yield return TimeYields.WaitSeconds(UITimer, 0.25);


                if (EnemyHPCounter.Count <= 0)
                {
                    _battleOver = true;
                    Enemy.Die();
                }

                if (PlayerHPCounter.Count <= 0)
                {
                    _battleOver = true;
                    _drillBattler.Die();
                }
            }

            Enemy.OnHit -= Enemy_OnHit;
            _drillBattler.OnHit -= Player_OnHit;
            _battleIntro.ExitBattle(Enemy, PlayerHPCounter.Count>0);
        }

        private IEnumerable<IEnumerable<Action>> DecisionPhase()
        {
            var showActionBar = _actionSelectorBar.Show().AsCoroutine();
            var showEnemyBar = _enemyActionBar.Show(Enemy).AsCoroutine();

            yield return showActionBar.Combine(showEnemyBar); // show and wait for decision
            DefaultMachinery.AddBasicMachine(_enemyActionBar.Hide()); // hide and go to action phase
            yield return TimeYields.WaitOneFrameX;
        }

        private IEnumerable<IEnumerable<Action>> ActionPhase()
        {
            // Play player action
            ActionNameCaption.Text = _actionSelectorBar.SelectedAction.Name;
            ActionNameCaption.DefaultMaterial = _actionSelectorBar.GetActionMaterial();

            yield return PlayAction().AsCoroutine();

            ActionNameCaption.Text = "";

            if (EnemyHPCounter.Count <= 0) yield break;
            // Wait a few
            yield return TimeYields.WaitSeconds(UITimer, 1);

            // Play enemy action
            EnemyActionNameCaption.Text = _enemyActionBar.SelectedAction.Name;
            EnemyActionNameCaption.DefaultMaterial = _enemyActionBar.GetActionMaterial();

            yield return PlayEnemyAction().AsCoroutine();

            EnemyActionNameCaption.Text = "";
        }

        private IEnumerable<IEnumerable<Action>> PlayEnemyAction()
        {
            if (!string.IsNullOrWhiteSpace(_enemyActionBar.SelectedAction.Animation)) Enemy.PlayAnim(_enemyActionBar.SelectedAction.Animation);

            HitEffect effect = null;
            if (_enemyActionBar.SelectedAction.SkillEffect != null)
            {
                yield return TimeYields.WaitSeconds(UITimer, _enemyActionBar.SelectedAction.EffectDelayInSeconds);
                var effectPool = _hitPoolManager.GetEffect(_enemyActionBar.SelectedAction.SkillEffect);
                if (effectPool.TryGetFromPool(out effect))
                {
                    effect.Target = _drillBattler;
                    effect.TotalDamage = _enemyActionBar.SelectedAction.Power;
                }
            }

            yield return TimeYields.WaitSeconds(UITimer, _enemyActionBar.SelectedAction.DurationInSeconds);

            if (effect != null)
            {
                effect.EndEffect();
            }
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
                    effect.Target = Enemy;
                    effect.TotalDamage = _actionSelectorBar.SelectedAction.CalculateDamage(_playerStats.DrillPower,
                        _playerStats.JetpackBattery, _playerStats.MaxHP, Enemy.Element);
                }
            }

            yield return TimeYields.WaitSeconds(UITimer, _actionSelectorBar.SelectedAction.DurationInSeconds);

            if (_actionSelectorBar.SelectedAction.SelfDamage > 0)
            {
                _drillBattler.Hit(1);
                yield return TimeYields.WaitSeconds(UITimer, 1);
            }

            if (effect != null)
            {
                effect.EndEffect();
            }
        }
    }
}
