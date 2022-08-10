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
            var (element, cost) = info;
            ElementSprite.sprite = GetSpriteByElement(element);
            CostNumber.Number = cost;

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
                BattleElement.Fire => UIElementGlyphs.FireSprite,
                BattleElement.Ice => UIElementGlyphs.IceSprite,
                BattleElement.Bio => UIElementGlyphs.BioSprite,
                BattleElement.Crystal => UIElementGlyphs.CrystalSprite,
                _ => UIElementGlyphs.NeutralSprite
            };
        }
    }
}
