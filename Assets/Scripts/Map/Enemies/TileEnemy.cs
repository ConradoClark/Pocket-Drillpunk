using System.Collections.Generic;
using Licht.Impl.Debug;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TileEnemy : EffectPoolable, ITileStateExtension
    {
        public BaseTile Tile;

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
