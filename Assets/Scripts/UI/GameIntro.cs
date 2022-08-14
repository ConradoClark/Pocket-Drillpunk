using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class GameIntro : BaseUIObject
    {
        public Transform UI;
        public SpriteRenderer Intro;
        public AudioSource GameSong;

        private MapGenerator _mapGenerator;
        private Player _player;
        protected override void OnAwake()
        {
            base.OnAwake();
            _mapGenerator = SceneObject<MapGenerator>.Instance();
            _player = SceneObject<Player>.Instance();
        }
        private void OnEnable()
        {
            UI.gameObject.SetActive(false);
            _player.PhysicsObject.enabled = false;
            _mapGenerator.OnPopulate += OnPopulate;
        }

        private IEnumerable<IEnumerable<Action>> FadeOutIntro()
        {
            yield return TimeYields.WaitSeconds(UITimer, 2f);

            GameSong.Play();
            UI.gameObject.SetActive(true);
            yield return Intro.GetAccessor()
                .Color
                .A
                .SetTarget(0f)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            _player.PhysicsObject.enabled = true;
        }

        private void OnPopulate()
        {
            DefaultMachinery.AddBasicMachine(FadeOutIntro());
            _mapGenerator.OnPopulate -= OnPopulate;
        }

        private void OnDisable()
        {
            _mapGenerator.OnPopulate -= OnPopulate;
        }
    }
}
