using System.Collections.Generic;
using Assets.Scripts.Battle;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.Map.Enemies
{
    public class TileEnemy : EffectPoolable, ITileStateExtension
    {
        public ScriptPrefab EnemyBattler;
        public BaseTile Tile;
        private BattleIntro _battleIntro;

        protected override void OnAwake()
        {
            base.OnAwake();
            _battleIntro = SceneObject<BattleIntro>.Instance();
        }

        private void OnDisable()
        {
            if (Tile == null) return;
            Tile.OnRelease -= Tile_OnRelease;
            Tile.OnBreak -= Tile_OnBreak;
        }

        public void HookOnTileRelease()
        {
            Tile.OnRelease += Tile_OnRelease;
        }

        private void Tile_OnRelease()
        {
            EndEffect();
        }

        public override void OnActivation()
        {
        }

        public void Setup()
        {
            Tile.OnBreak += Tile_OnBreak;
        }

        private void Tile_OnBreak()
        {
            Debug.Log($"Tile with enemy broken! {Tile.gameObject.name}");
            _battleIntro.EnterBattle(EnemyBattler);

            Tile.OnBreak -= Tile_OnBreak;
        }

        public bool Dirty => false;

        public void LoadState(Dictionary<string, object> dict)
        {
            
        }

        public void SaveState(Dictionary<string, object> dict)
        {
            
        }
    }
}
