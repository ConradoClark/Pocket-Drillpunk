using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Battle;
using Assets.Scripts.Drill;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI
{
    public class UIActionSelectorBar : BaseUIObject
    {
        [Header("Input")]
        public ScriptInput MoveInput;

        public ScriptInput ConfirmInput;

        [Header("Animation")]
        public float HiddenPosition;
        public float BarPosition;
        public float AnimDurationInSeconds;

        [Header("TextRenderer")]
        public UITextRenderer ActionText;

        [Header("Materials")]
        public Material NeutralActionMaterial;
        public Material FireActionMaterial;
        public Material IceActionMaterial;
        public Material BioActionMaterial;
        public Material CrystalActionMaterial;

        public UIActionGemCostBar Cost1;
        public UIActionGemCostBar Cost2;
        public UIActionGemCostBar Cost3;

        public SpriteRenderer UpArrow;
        public SpriteRenderer DownArrow;

        public DrillSkill SelectedAction { get; private set; }
        private int _selectedActionIndex;
        private PlayerStats _playerStats;

        private InputAction _moveAction;
        private InputAction _confirmAction;

        protected override void OnAwake()
        {
            base.OnAwake();
            _playerStats = SceneObject<PlayerStats>.Instance(true);
            SelectedAction = _playerStats.Moves[1];
            _selectedActionIndex = 1;

            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _moveAction = playerInput.actions[MoveInput.ActionName];
            _confirmAction = playerInput.actions[ConfirmInput.ActionName];
        }

        public Material GetActionMaterial()
        {
            switch (SelectedAction.Element)
            {
                case BattleElement.Neutral: return NeutralActionMaterial;
                case BattleElement.Fire: return FireActionMaterial;
                case BattleElement.Ice: return IceActionMaterial;
                case BattleElement.Bio: return BioActionMaterial;
                case BattleElement.Crystal: return CrystalActionMaterial;
                default: return NeutralActionMaterial;
            }
        }

        public IEnumerable<IEnumerable<Action>> Show()
        {
            UpArrow.gameObject.SetActive(_selectedActionIndex != 0);
            DownArrow.gameObject.SetActive(_selectedActionIndex != _playerStats.Moves.Count - 1);

            ActionText.Text = SelectedAction.Name;
            ActionText.DefaultMaterial = GetActionMaterial();

            var main = transform.GetAccessor()
                .Position
                .X
                .SetTarget(BarPosition)
                .Over(AnimDurationInSeconds)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(UITimer)
                .Build();


            DefaultMachinery.AddUniqueMachine("costs", UniqueMachine.UniqueMachineBehaviour.Wait, ReloadCosts(false));

            yield return main;
            yield return HandleInputs().AsCoroutine();
            yield return Hide().AsCoroutine();
        }

        private IEnumerable<IEnumerable<Action>> ReloadCosts(bool quick)
        {
            yield return TimeYields.WaitOneFrameX;
            var costs = GetCosts();

            if (costs.Count == 0)
            {
                yield return Cost1.Hide(quick).AsCoroutine()
                    .Combine(Cost2.Hide(quick).AsCoroutine())
                    .Combine(Cost3.Hide(quick).AsCoroutine());
                yield break;
            }

            var actionCosts = Cost1.Show(costs[0], quick).AsCoroutine();
            actionCosts = actionCosts.Combine(costs.Count >= 2 ? Cost2.Show(costs[1], quick).AsCoroutine() : Cost2.Hide(quick).AsCoroutine());
            actionCosts = actionCosts.Combine(costs.Count >= 3 ? Cost3.Show(costs[2], quick).AsCoroutine() : Cost3.Hide(quick).AsCoroutine());

            yield return actionCosts;
        }

        private IEnumerable<IEnumerable<Action>> HandleInputs()
        {
            while (!_confirmAction.WasPerformedThisFrame())
            {
                if (_moveAction.WasPerformedThisFrame())
                {
                    var move = _moveAction.ReadValue<Vector2>();
                    
                    switch (move.y)
                    {
                        case > 0 when _selectedActionIndex != 0:
                            _selectedActionIndex--;
                            SelectedAction = _playerStats.Moves[_selectedActionIndex];
                            DefaultMachinery.AddUniqueMachine("costs", UniqueMachine.UniqueMachineBehaviour.Wait, ReloadCosts(true));
                            break;
                        case < 0 when _selectedActionIndex != _playerStats.Moves.Count - 1:
                            _selectedActionIndex++;
                            SelectedAction = _playerStats.Moves[_selectedActionIndex];
                            DefaultMachinery.AddUniqueMachine("costs", UniqueMachine.UniqueMachineBehaviour.Wait, ReloadCosts(true));
                            break;
                    }

                    ActionText.Text = SelectedAction.Name;
                    ActionText.DefaultMaterial = GetActionMaterial();
                    UpArrow.gameObject.SetActive(_selectedActionIndex != 0);
                    DownArrow.gameObject.SetActive(_selectedActionIndex != _playerStats.Moves.Count - 1);
                }

                yield return TimeYields.WaitOneFrameX;
            }


        }

        private List<(BattleElement element, int cost)> GetCosts()
        {
            var result = new List<(BattleElement element, int cost)>();
            var costs = new[]
            {
                SelectedAction.NeutralCost,
                SelectedAction.FireCost,
                SelectedAction.IceCost,
                SelectedAction.BioCost,
                SelectedAction.CrystalCost
            };

            for (var i = 0; i < costs.Length; i++)
            {
                if (costs[i] > 0)
                {
                    result.Add((GetBattleElementByIndex(i), costs[i]));
                }

                if (result.Count >= 3) break;
            }

            return result;
        }

        private BattleElement GetBattleElementByIndex(int i)
        {
            return i switch
            {
                0 => BattleElement.Neutral,
                1 => BattleElement.Fire,
                2 => BattleElement.Ice,
                3 => BattleElement.Bio,
                4 => BattleElement.Crystal,
                _ => BattleElement.Neutral
            };
        }

        public IEnumerable<IEnumerable<Action>> Hide()
        {
           var hideBar = transform.GetAccessor()
                .Position
                .X
                .SetTarget(HiddenPosition)
                .Over(AnimDurationInSeconds)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(UITimer)
                .Build();

           var hideCosts = Cost1.Hide(false).AsCoroutine()
               .Combine(Cost2.Hide(false).AsCoroutine())
               .Combine(Cost3.Hide(false).AsCoroutine());

           yield return hideBar.Combine(hideCosts);
        }

    }
}
