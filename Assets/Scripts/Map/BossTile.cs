using System;
using System.Collections.Generic;
using Assets.Scripts.Battle;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

namespace Assets.Scripts.Map
{
    public class BossTile : DirtTile
    {
        private BattleIntro _battleIntro;

        protected override void OnAwake()
        {
            base.OnAwake();
            _battleIntro = SceneObject<BattleIntro>.Instance();
        }

        protected override IEnumerable<IEnumerable<Action>> OnBreakEffect()
        {
            if (ExplosionEffect.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position;
            }
            _battleIntro.EnterBattle(MapGenerator.CurrentLevelDefinition.BossBattler);

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
