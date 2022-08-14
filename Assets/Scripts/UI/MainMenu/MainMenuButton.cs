using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuButton : BaseUIObject
    {
        public bool Horizontal;
        public int Index;
        public ScriptInput MouseClickInput;
        public ScriptInput ConfirmInput;
        public ScriptInput MousePosInput;

        public Sprite DefaultSprite;
        public Sprite SelectedSprite;

        public Vector3 SelectorOffset;
        public SpriteRenderer Selector;

        public RuntimeAnimatorController DefaultSelectorAnimatorController;
        public RuntimeAnimatorController ActiveSelectorAnimatorController;
        public Animator SelectorAnimator;
        public bool StartsSelected;
        public MainMenuAction Action;
        public MainMenuButtonGroup ButtonGroup;

        private SpriteRenderer _spriteRenderer;
        private ClickableObjectMixin _clickable;
        private InputAction _confirmInput;
        public bool IsSelected { get; private set; }
        private bool _enabled;

        protected override void OnAwake()
        {
            base.OnAwake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _clickable = new ClickableObjectMixinBuilder(this, MousePosInput, MouseClickInput).Build();
            _confirmInput = playerInput.actions[ConfirmInput.ActionName];

            if (StartsSelected) Select();
        }

        private void OnEnable()
        {
            ButtonGroup.AddButton(this, Index);
            _enabled = true;

            DefaultMachinery.AddBasicMachine(_clickable.HandleHover(() =>
                {
                    if (ButtonGroup.IsExecutingAction) return;
                    Select();
                    ButtonGroup.DeselectOthers(this);
                },
                () =>
                {
                }));

           DefaultMachinery.AddBasicMachine(HandleClick());
        }

        public virtual void Select()
        {
            _spriteRenderer.sprite = SelectedSprite;
            IsSelected = true;
            Selector.transform.position = transform.position + SelectorOffset;
        }

        public virtual void Deselect()
        {
            _spriteRenderer.sprite = DefaultSprite;
            IsSelected = false;
        }

        private void OnDisable()
        {
            _enabled = false;
            ButtonGroup.RemoveButton(this);
        }

        private IEnumerable<IEnumerable<Action>> HandleClick()
        {
            while (_enabled)
            {
                if (!IsSelected)
                {
                    yield return TimeYields.WaitOneFrameX;
                    continue;
                }

                if ((_confirmInput.WasPerformedThisFrame() || _clickable.WasClickedThisFrame()) && Action!=null)
                {
                    ButtonGroup.IsExecutingAction = true;
                    SelectorAnimator.runtimeAnimatorController = ActiveSelectorAnimatorController;
                    yield return Action.PerformAction().AsCoroutine();
                    SelectorAnimator.runtimeAnimatorController = DefaultSelectorAnimatorController;
                    ButtonGroup.IsExecutingAction = false;
                }

                yield return TimeYields.WaitOneFrameX;
            }
            
        }
    }
}
