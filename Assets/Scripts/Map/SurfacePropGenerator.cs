using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class SurfacePropGenerator : MonoBehaviour, ITilePropGenerator
    {
        public BaseTile Tile;
        public string Name;
        public float OriginalWeight;
        public float Weight { get; set; }
        public ScriptPrefab Prefab;
        public bool Force;

        public Vector2Int[] AllowedDirections;

        private MapManager _manager;
        private SurfacePropPool _pool;

        private void Awake()
        {
            Weight = OriginalWeight;
            _manager = SceneObject<MapManager>.Instance();
            var poolManager = SceneObject<SurfacePropPoolManager>.Instance();
            _pool = poolManager.GetEffect(Prefab);
        }

        public void Populate(string seed)
        {
            var rng = new System.Random($"{seed}_{Name}".GetHashCode());

            if (rng.NextDouble() > Weight) return;

            var sides = new[]
            {
                (Vector2Int.right, GetAdjacentTile(Vector2Int.right)),
                (Vector2Int.left,GetAdjacentTile(Vector2Int.left)),
                (Vector2Int.up,GetAdjacentTile(Vector2Int.up)),
                (Vector2Int.down,GetAdjacentTile(Vector2Int.down))
            }.Where(s => AllowedDirections.Contains(s.Item1) && (Force || !Tile.IsOccupiedByProp(s.Item1) &&  s.Item2 == null)).ToArray();

            if (sides.Length == 0) return;

            var chosenSide = sides[rng.Next(0, sides.Length)];
            if (_pool.TryGetFromPool(out var prop))
            {
                var side = chosenSide.Item1;
                prop.Tile = Tile;
                prop.HookOnTileRelease();
                prop.SetPositionAndRotationBasedOnSide(side);
                Tile.AddTileExtension(prop, side);
            }
        }

        private BaseTile GetAdjacentTile(Vector2Int direction)
        {
            if (_manager.ActiveTiles.ContainsKey(Tile.Position + direction))
            {
                return _manager.ActiveTiles[Tile.Position + direction];
            }

            return null;
        }

    }
}
