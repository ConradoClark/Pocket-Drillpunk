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

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuButtonGroup : BaseUIObject
    {
        public ScriptInput MoveInput;

        private List<MainMenuButton> _buttons;
        private bool _enabled;
        private InputAction _moveAction;
        private int _latestHorizontalSelected;

        private Dictionary<int, MainMenuButton> _dict;


        public bool IsExecutingAction { get; set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _moveAction = playerInput.actions[MoveInput.ActionName];
        }

        public void AddButton(MainMenuButton button, int index)
        {
            _dict ??= new Dictionary<int, MainMenuButton>();
            _dict[index] = button;

            _buttons = _dict.OrderBy(d => d.Key).Select(d => d.Value).ToList();
        }

        public void RemoveButton(MainMenuButton button)
        {
            _buttons?.Remove(button);
        }

        public void DeselectOthers(MainMenuButton selectedButton)
        {
            if (_buttons == null) return;
            foreach (var button in _buttons)
            {
                if (button == selectedButton) continue;
                button.Deselect();
            }
        }

        private void OnEnable()
        {
            _enabled = true;
            DefaultMachinery.AddBasicMachine(HandleInput());
        }

        private void OnDisable()
        {
            _enabled = false;
        }

        private IEnumerable<IEnumerable<Action>> HandleInput()
        {
            while (_buttons == null)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            while (_enabled)
            {
                if (_moveAction.WasPerformedThisFrame() && !IsExecutingAction)
                {
                    var currentlySelectedButton = _buttons.FindIndex(button => button.IsSelected);
                    var move = _moveAction.ReadValue<Vector2>();

                    if (move.x > 0 && _buttons[currentlySelectedButton].Horizontal && currentlySelectedButton < _buttons.Count - 1)
                    {
                        currentlySelectedButton++;

                        if (_buttons[currentlySelectedButton].Horizontal)
                        {
                            _latestHorizontalSelected = currentlySelectedButton;
                            _buttons[currentlySelectedButton].Select();
                            DeselectOthers(_buttons[currentlySelectedButton]);
                        }
                    }

                    if (move.x < 0 && currentlySelectedButton > 0)
                    {
                        currentlySelectedButton--;

                        if (_buttons[currentlySelectedButton].Horizontal)
                        {
                            _latestHorizontalSelected = currentlySelectedButton;
                            _buttons[currentlySelectedButton].Select();
                            DeselectOthers(_buttons[currentlySelectedButton]);
                        }
                    }

                    if (move.y > 0 && currentlySelectedButton > 0)
                    {
                        do
                        {
                            currentlySelectedButton--;
                        } while (currentlySelectedButton > 0 && _buttons[currentlySelectedButton].Horizontal);

                        if (!_buttons[currentlySelectedButton].Horizontal)
                        {
                            _buttons[currentlySelectedButton].Select();
                            DeselectOthers(_buttons[currentlySelectedButton]);
                        }
                        else
                        {
                            _buttons[_latestHorizontalSelected].Select();
                            DeselectOthers(_buttons[_latestHorizontalSelected]);
                        }
                    }

                    else if (move.y < 0 && currentlySelectedButton < _buttons.Count - 1)
                    {
                        do
                        {
                            currentlySelectedButton++;
                        } while (currentlySelectedButton < _buttons.Count - 1 && _buttons[currentlySelectedButton].Horizontal);

                        if (!_buttons[currentlySelectedButton].Horizontal)
                        {
                            _buttons[currentlySelectedButton].Select();
                            DeselectOthers(_buttons[currentlySelectedButton]);
                        }
                        else
                        {
                            _buttons[_latestHorizontalSelected].Select();
                            DeselectOthers(_buttons[_latestHorizontalSelected]);
                        }
                    }
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }

        public IEnumerable<IEnumerable<Action>> HideMenu()
        {
            var selector = _buttons.First().Selector;

            var hideAction = selector.transform.GetAccessor()
                .Position
                .X
                .Increase(10)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            foreach (var button in _buttons)
            {
                hideAction = hideAction.Combine(button.transform.GetAccessor()
                    .Position
                    .X
                    .Increase(10)
                    .Over(1f)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                    .UsingTimer(UITimer)
                    .Build());
            }

            yield return hideAction;

            while (_buttons.Count > 0)
            {
                _buttons[0].enabled = false;
            }

            enabled = false;
        }

    }
}
