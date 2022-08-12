using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI
{
    public class UILevelUpMenu : BaseUIObject
    {
        public UITextRenderer TextRenderer;
        public SpriteRenderer Selector;
        public ScriptInput MoveInput;
        public ScriptInput ConfirmInput;
        public UILevelUpOption[] Options;
        
        private UILevelUpOption _selectedOption;
        private int _selectedOptionIndex;
        private InputAction _moveAction;
        private InputAction _confirmAction;
        private bool _initialized;
        private Player _player; 

        protected override void OnAwake()
        {
            if (_initialized) return;
            _initialized = true;
            base.OnAwake();
            _selectedOptionIndex = 0;
            _selectedOption = Options[_selectedOptionIndex];
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _confirmAction = playerInput.actions[ConfirmInput.ActionName];
            _moveAction = playerInput.actions[MoveInput.ActionName];
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

            yield return TimeYields.WaitSeconds(UITimer, 1);
            yield return Choose().AsCoroutine();
            yield return Hide().AsCoroutine();
        }

        private IEnumerable<IEnumerable<Action>> Choose()
        {
            while (!_confirmAction.WasPerformedThisFrame())
            {
                if (_moveAction.WasPerformedThisFrame())
                {
                    var move = _moveAction.ReadValue<Vector2>();

                    if (move.x > 0)
                    {
                        _selectedOptionIndex = Math.Clamp(_selectedOptionIndex + 1, 0, Options.Length - 1);
                        _selectedOption = Options[_selectedOptionIndex];
                        Selector.transform.position = _selectedOption.transform.position;
                        TextRenderer.Text = _selectedOption.Text;
                        TextRenderer.DefaultMaterial = _selectedOption.TextMaterial;
                    }
                    else if (move.x < 0)
                    {
                        _selectedOptionIndex = Math.Clamp(_selectedOptionIndex - 1, 0, Options.Length - 1);
                        _selectedOption = Options[_selectedOptionIndex];
                        Selector.transform.position = _selectedOption.transform.position;
                        TextRenderer.Text = _selectedOption.Text;
                        TextRenderer.DefaultMaterial = _selectedOption.TextMaterial;
                    }
                }
                yield return TimeYields.WaitOneFrameX;
            }

            _player.LevelUp(_selectedOption.LevelUpOption);
        }

        public IEnumerable<IEnumerable<Action>> Hide()
        {
            _player.Unblock();
            yield return TimeYields.WaitSeconds(UITimer, 0.25);
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
