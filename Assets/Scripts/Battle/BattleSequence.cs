using System;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

namespace Assets.Scripts.Battle
{
    public class BattleSequence : BaseUIObject
    {
        private bool _battleOver;

        private UIActionSelectorBar _actionSelectorBar;
        protected override void OnAwake()
        {
            base.OnAwake();
            _actionSelectorBar = SceneObject<UIActionSelectorBar>.Instance(true);
        }

        public void StartBattle()
        {
            _battleOver = false;
            DefaultMachinery.AddBasicMachine(Battle());
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
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
