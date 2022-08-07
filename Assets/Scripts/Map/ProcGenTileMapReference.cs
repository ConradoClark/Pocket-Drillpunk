using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class ProcGenTileMapReference : ProcGenMapReference
    {
        public ScriptPrefab BaseTile;
        private BaseTilePool _tilePool;
        protected override void OnAwake()
        {
            base.OnAwake();
            if (BaseTile == null) return;

            var tilePoolManager = SceneObject<BaseTilePoolManager>.Instance();
            _tilePool = tilePoolManager.GetEffect(BaseTile);
        }

        public override Vector2Int RefSize { get; } = Vector2Int.one;

        public override void Populate(string seed, Vector2Int position)
        {
            PopulatedPositions.Add(position);

            if (BaseTile == null) return;
            if (_tilePool.TryGetFromPool(out var tile))
            {
                tile.Component.transform.position = Grid.GetCellCenterWorld((Vector3Int) position);
                tile.Position = position;
            }
        }
    }
}
