using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.UI.MainMenu
{
    public class StartGameAction: MainMenuAction
    {
        public Color TargetColor;
        public SpriteRenderer DrillPunkCover;
        public SpriteRenderer DrillPunkCoverSmoke;
        public MainMenuButtonGroup ButtonGroup;

        public Transform LevelSelectionMenu;

        public override IEnumerable<IEnumerable<Action>> PerformAction()
        {
            var colorize= DrillPunkCover.GetAccessor()
                .Material("_Colorize")
                .AsColor()
                .ToColor(TargetColor)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            var fade = DrillPunkCoverSmoke
                .GetAccessor()
                .Color
                .A
                .SetTarget(0)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            yield return colorize.Combine(fade).Combine(ButtonGroup.HideMenu().AsCoroutine());

            LevelSelectionMenu.gameObject.SetActive(true);
            yield return LevelSelectionMenu.transform.GetAccessor()
                .Position
                .X
                .SetTarget(0)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(UITimer)
                .Build();
        }
    }
}
