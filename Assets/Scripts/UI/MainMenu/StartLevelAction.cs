using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.MainMenu
{
    public class StartLevelAction : MainMenuAction
    {
        public LevelDefinition CurrentLevelDefinition;
        public LevelDefinition TargetLevel;
        public Transform TargetTransform;
        public Transform SelectorTransform;

        public SpriteRenderer IntroLoop;

        public override IEnumerable<IEnumerable<Action>> PerformAction()
        {
            CurrentLevelDefinition.CopyFrom(TargetLevel);

            var targetMove = TargetTransform.GetAccessor()
                .Position
                .X
                .Increase(10)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            var selectorMove = SelectorTransform.GetAccessor()
                .Position
                .X
                .Increase(10)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            yield return targetMove.Combine(selectorMove);

            IntroLoop.enabled = true;

            yield return TimeYields.WaitMilliseconds(UITimer, 500);

            DefaultMachinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene("Scenes/Game");
            });

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
