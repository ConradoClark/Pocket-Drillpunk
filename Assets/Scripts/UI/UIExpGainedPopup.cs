using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Inventory;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TimeYields = Licht.Impl.Orchestration.TimeYields;

namespace Assets.Scripts.UI
{
    public class UIExpGainedPopup : BaseUIObject
    {
        public ScriptInput ConfirmInput;
        private InputAction _confirmAction;
        private Player _player;

        public Counter ExpGainedCounter;
        public Counter ToNextLevelCounter;

        private bool _initialized;

        protected override void OnAwake()
        {
            if (_initialized) return;
            _initialized = true;
            base.OnAwake();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _confirmAction = playerInput.actions[ConfirmInput.ActionName];
            _player = SceneObject<Player>.Instance(true);
        }

        public IEnumerable<IEnumerable<Action>> Show()
        {
            if (!_initialized)
            {
                OnAwake();
            }

            _player.Block();
            gameObject.SetActive(true);
            transform.localScale = new Vector3(1, 0.1f, 1);
            yield return transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(1f)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            yield return TimeYields.WaitSeconds(UITimer, 2);
            yield return Calculate().AsCoroutine();
            yield return Hide().AsCoroutine();
        }

        public IEnumerable<IEnumerable<Action>> Calculate()
        {
            while (ExpGainedCounter.Count > 0)
            {
                ExpGainedCounter.Count--;
                ToNextLevelCounter.Count--;
                yield return TimeYields.WaitMilliseconds(UITimer, 100);
            }
        }

        public IEnumerable<IEnumerable<Action>> Hide()
        {
            _player.Unblock();
            yield return TimeYields.WaitSeconds(UITimer, 1);
            yield return transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(0.1f)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();
            gameObject.SetActive(false);
        }
    }
}
