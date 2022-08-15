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
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class UIVictoryPopup : BaseUIObject
    {
        public ScriptBasicMachinery PostUpdate;
        public ScriptInput ConfirmInput;
        private Player _player;
        private bool _initialized;
        private InputAction _confirmAction;

        protected override void OnAwake()
        {
            if (_initialized) return;
            _initialized = true;
            base.OnAwake();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _player = SceneObject<Player>.Instance(true);
            _confirmAction = playerInput.actions[ConfirmInput.ActionName];
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

            yield return TimeYields.WaitSeconds(UITimer, 0.25f);

            yield return Hide().AsCoroutine();
        }

        public IEnumerable<IEnumerable<Action>> Hide()
        {
            while (!_confirmAction.WasPerformedThisFrame())
            {
                yield return TimeYields.WaitOneFrameX;
            }

            PostUpdate.Machinery.FinalizeWith(() =>
            {

            });

            yield return TimeYields.WaitOneFrameX;

            DefaultMachinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene("Scenes/MainMenu");
            });

        }
    }
}
