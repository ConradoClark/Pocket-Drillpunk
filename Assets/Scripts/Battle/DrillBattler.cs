using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class DrillBattler : BaseUIObject
    {
        public Vector3 BattlerPosition;

        private Player _player;
        private Camera _gameCamera;
        private Camera _uiCamera;

        protected override void OnAwake()
        {
            base.OnAwake();
            _player = SceneObject<Player>.Instance(true);
            _gameCamera = SceneObject<GameCamera>.Instance().Camera;
            _uiCamera = SceneObject<UICamera>.Instance().Camera;
        }

        public IEnumerable<IEnumerable<Action>> ActivateAndMoveToPosition()
        {
            gameObject.SetActive(true);

            var playerViewportPos = _gameCamera.WorldToViewportPoint(_player.transform.position);
            var uiPos = _uiCamera.ViewportToWorldPoint(playerViewportPos);

            transform.position = new Vector3(uiPos.x, uiPos.y, 0);
            yield return transform.GetAccessor()
                .Position
                .ToPosition(BattlerPosition)
                .Over(4f)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(UITimer)
                .Build();

        }
    }
}
