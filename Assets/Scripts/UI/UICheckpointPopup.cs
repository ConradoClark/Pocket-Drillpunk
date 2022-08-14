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
    public class UICheckpointPopup : BaseUIObject
    {
        public AudioSource CheckpointSound;
        private Player _player;
        private bool _initialized;

        protected override void OnAwake()
        {
            if (_initialized) return;
            _initialized = true;
            base.OnAwake();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
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
            CheckpointSound.Play();
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
            yield return TimeYields.WaitSeconds(UITimer, 1);
            _player.Unblock();
            yield return transform.GetAccessor()
                .LocalScale
                .Y
                .SetTarget(0.1f)
                .Over(0.25f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();
            gameObject.SetActive(false);
        }
    }
}
