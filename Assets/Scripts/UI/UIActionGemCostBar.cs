using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Battle;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIActionGemCostBar : BaseUIObject
    {
        [Header("Animation")]
        public float HiddenPosition;
        public float BarPosition;
        public float QuickAnimDurationInSeconds;
        public float AnimDurationInSeconds;

        public UIElementGlyphs UIElementGlyphs;
        public SpriteRenderer ElementSprite;
        public UINumberRenderer CostNumber;



        public IEnumerable<IEnumerable<Action>> Show((BattleElement element, int cost) info, bool quick)
        {
            ElementSprite.sprite = GetSpriteByElement(info.element);
            CostNumber.Number = info.cost;

            yield return transform.GetAccessor()
                .Position
                .X
                .SetTarget(BarPosition)
                .Over(quick? QuickAnimDurationInSeconds : AnimDurationInSeconds)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(UITimer)
                .Build();
        }

        public IEnumerable<IEnumerable<Action>> Hide(bool quick)
        {
            yield return transform.GetAccessor()
                .Position
                .X
                .SetTarget(HiddenPosition)
                .Over(quick ? QuickAnimDurationInSeconds : AnimDurationInSeconds)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(UITimer)
                .Build();
        }

        private Sprite GetSpriteByElement(BattleElement element)
        {
            return element switch
            {
                BattleElement.Neutral => UIElementGlyphs.NeutralSprite,
                BattleElement.Fire => UIElementGlyphs.NeutralSprite,
                BattleElement.Ice => UIElementGlyphs.NeutralSprite,
                BattleElement.Bio => UIElementGlyphs.NeutralSprite,
                BattleElement.Crystal => UIElementGlyphs.NeutralSprite,
                _ => UIElementGlyphs.NeutralSprite
            };
        }
    }
}
