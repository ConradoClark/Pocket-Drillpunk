using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Inventory;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Drill
{
    public class DrillCollector : BaseGameObject
    {
        public SpriteRenderer SpriteRenderer;
        public Color BlinkColor;
        public LichtPhysicsCollisionDetector CollisionDetector;
        private LichtPhysics _physics;
        private bool _enabled;

        public AudioSource CollectSound;

        protected override void OnAwake()
        {
            base.OnAwake();
            _physics = this.GetLichtPhysics();
        }

        private void OnEnable()
        {
            _enabled = true;
            DefaultMachinery.AddBasicMachine(HandleCollect());
        }

        private void OnDisable()
        {
            _enabled = false;
        }

        private IEnumerable<IEnumerable<Action>> HandleCollect()
        {
            while (_enabled)
            {
                Collectable collectable = null;
                var trigger = CollisionDetector.Triggers.FirstOrDefault(t => t.TriggeredHit &&
                                                                             _physics.TryGetPhysicsObjectByCollider(t.Collider, out var target) && target.TryGetCustomObject(out collectable));

                if (trigger.TriggeredHit && !collectable.Collected)
                {
                    CollectSound.pitch = 0.9f + Random.value * 0.2f;
                    CollectSound.Play();
                    collectable.Collect();
                    DefaultMachinery.AddUniqueMachine("collectFlashEffect",
                        UniqueMachine.UniqueMachineBehaviour.Replace, CollectFlashEffect());
                }

                yield return TimeYields.WaitOneFrameX;
            }
            
        }

        private IEnumerable<IEnumerable<Action>> CollectFlashEffect()
        {
            SpriteRenderer.color = Color.white;

            yield return SpriteRenderer.GetAccessor()
                .Color
                .ToColor(BlinkColor)
                .Over(0.25f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return SpriteRenderer.GetAccessor()
                .Color
                .ToColor(Color.white)
                .Over(0.10f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(GameTimer)
                .Build();
        }
    }
}
