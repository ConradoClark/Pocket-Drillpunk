using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Map
{
    public class CheckpointTerminal : BaseGameObject
    {
        public Vector3 CheckpointOffset;
        public float ActivationDistance;
        public Transform Marker;
        public ScriptInput ConfirmInput;

        private Player _player;
        private UICheckpointPopup _checkpointPopup;
        private bool _enabled;
        private InputAction _confirmAction;

        protected override void OnAwake()
        {
            base.OnAwake();
            _player = SceneObject<Player>.Instance(true);
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _confirmAction = playerInput.actions[ConfirmInput.ActionName];
            _checkpointPopup = SceneObject<UICheckpointPopup>.Instance(true);
        }

        private void OnEnable()
        {
            _enabled = true;
            DefaultMachinery.AddBasicMachine(HandleTerminal());
        }

        private void OnDisable()
        {
            _enabled = false;
        }

        private IEnumerable<IEnumerable<Action>> HandleTerminal()
        {
            while (_enabled)
            {
                if (Vector2.Distance(_player.transform.position, transform.position) <= ActivationDistance)
                {
                    Marker.gameObject.SetActive(true);
                    while (Vector2.Distance(_player.transform.position, transform.position) <= ActivationDistance)
                    {
                        if (_confirmAction.WasPerformedThisFrame())
                        {
                            _player.CheckpointPosition = transform.position + CheckpointOffset;
                            yield return _checkpointPopup.Show().AsCoroutine();
                        }
                        yield return TimeYields.WaitOneFrameX;
                    }
                }
                else
                {
                    Marker.gameObject.SetActive(false);
                    while (Vector2.Distance(_player.transform.position, transform.position) > ActivationDistance)
                    {
                        yield return TimeYields.WaitOneFrameX;
                    }
                }
            }
        }
    }
}
