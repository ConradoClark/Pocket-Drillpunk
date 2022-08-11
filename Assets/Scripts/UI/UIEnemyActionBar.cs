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
    public class UIEnemyActionBar : BaseUIObject
    {
        [Header("Animation")] public float HiddenPosition;
        public float BarPosition;
        public float AnimDurationInSeconds;

        public UINumberRenderer ActionValueRenderer;
        public SpriteRenderer ActionType;
        public Sprite AttackIcon;
        public Sprite ShieldIcon;
        public Sprite StunIcon;

        [Header("Materials")]
        public Material NeutralActionMaterial;
        public Material FireActionMaterial;
        public Material IceActionMaterial;
        public Material BioActionMaterial;
        public Material CrystalActionMaterial;


        public EnemySkill SelectedAction { get; set; }

        public IEnumerable<IEnumerable<Action>> Show(EnemyBattler enemy)
        {
            SelectedAction = enemy.SelectAction();
            UpdateBar();

            yield return transform.GetAccessor()
                .Position
                .X
                .SetTarget(BarPosition)
                .Over(AnimDurationInSeconds)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(UITimer)
                .Build();

        }

        public IEnumerable<IEnumerable<Action>> Hide()
        {
            yield return transform.GetAccessor()
                .Position
                .X
                .SetTarget(HiddenPosition)
                .Over(AnimDurationInSeconds)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();
        }

        private void UpdateBar()
        {
            ActionValueRenderer.Number = SelectedAction.Power > 0 ? SelectedAction.Power : SelectedAction.Shield;
            ActionType.sprite = SelectedAction.Power > 0 ? AttackIcon : SelectedAction.Shield > 0 ? ShieldIcon : StunIcon;
        }

        public Material GetActionMaterial()
        {
            switch (SelectedAction.Element)
            {
                case BattleElement.Neutral: return NeutralActionMaterial;
                case BattleElement.Fire: return FireActionMaterial;
                case BattleElement.Ice: return IceActionMaterial;
                case BattleElement.Bio: return BioActionMaterial;
                case BattleElement.Crystal: return CrystalActionMaterial;
                default: return NeutralActionMaterial;
            }
        }
    }
}
