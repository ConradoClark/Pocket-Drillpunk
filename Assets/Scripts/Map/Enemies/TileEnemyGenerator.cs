using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Generation;
using Licht.Interfaces.Generation;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Map.Enemies
{
    public class TileEnemyGenerator : MonoBehaviour, ITilePropGenerator, IGenerator<int,float>
    {
        [Serializable]
        public struct SpawnRate : IWeighted<float>
        {
            public ScriptPrefab Enemy;
            public float SpawnWeight;
            public float Weight => SpawnWeight;
        }

        public BaseTile Tile;
        public ScriptIdentifier Name;
        public float OriginalWeight;
        public float Weight { get; set; }
        public SpawnRate[] SpawnRates;
        private Dictionary<SpawnRate, TileEnemyPool> _pools;
        private System.Random _rng;
        private MapGenerator _mapGenerator;

        public bool HasMinDepth;
        public float MinDepth;

        public bool HasMaxDepth;
        public float MaxDepth;

        private void Awake()
        {
            Weight = OriginalWeight;
            var poolManager = SceneObject<TileEnemyPoolManager>.Instance();
            _pools = new Dictionary<SpawnRate, TileEnemyPool>();
            _mapGenerator = SceneObject<MapGenerator>.Instance();
            foreach (var spawn in SpawnRates)
            {
                _pools[spawn] = poolManager.GetEffect(spawn.Enemy);
            }
        }

        private void ManipulateWeight()
        {
            var rule = _mapGenerator.CurrentLevelDefinition.Rules.FirstOrDefault(r => r.TileIdentifier == Name);
            if (rule.Equals(default(MapRules.WeightRule))) return;

            Weight = rule.Weight;
        }

        public void Populate(string seed)
        {
            ManipulateWeight();
            if (Tile.IsOccupiedByProp(Vector2Int.one) || Weight == 0) return;
            if (HasMaxDepth && Tile.Position.y < MaxDepth) return;
            if (HasMinDepth && Tile.Position.y > MinDepth) return;
            if (_pools.Count == 0) return;

            Seed = $"{seed}_{Name}".GetHashCode();
            _rng = new System.Random(Seed);

            if (_rng.NextDouble() > Weight) return;

            TileEnemyPool pool;

            if (_pools.Count == 1)
            {
                pool = _pools.Values.First();
            }
            else
            {
                var randomDice = new WeightedDice<SpawnRate>(SpawnRates, this);
                pool = _pools[randomDice.Generate()];
            }

            if (pool.TryGetFromPool(out var enemy))
            {
                enemy.transform.position = Tile.transform.position;
                enemy.Tile = Tile;
                enemy.HookOnTileRelease();
                enemy.Setup();
                Tile.AddTileExtension(enemy, Vector2Int.one);
            }
        }

        public int Seed { get; set; }
        public float Generate()
        {
            return (float)_rng.NextDouble();
        }
    }
}
