using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TileBorderFixer : MonoBehaviour
    {
        public BaseTile Tile;
        public SpriteRenderer SpriteRenderer;

        [Header("Sprites")]
        public Sprite CenterSprite;
        public Sprite NorthSprite;
        public Sprite NorthEastSprite;
        public Sprite NorthWestSprite;
        public Sprite WestSprite;
        public Sprite EastSprite;
        public Sprite SouthSprite;
        public Sprite SouthEastSprite;
        public Sprite SouthWestSprite;

        private MapGenerator _generator;
        private MapManager _manager;

        private BaseTile _east;
        private BaseTile _west;
        private BaseTile _north;
        private BaseTile _south;

        private void Awake()
        {
            _generator = SceneObject<MapGenerator>.Instance();
            _manager = SceneObject<MapManager>.Instance();
        }

        private void OnEnable()
        {
            _generator.OnPopulate += OnPopulate;
        }
        private void OnDisable()
        {
            _generator.OnPopulate -= OnPopulate;
        }

        private void OnPopulate()
        {
            ClearBreakEvent();
            _east = GetAdjacentTile(Vector2Int.right);
            _west = GetAdjacentTile(Vector2Int.left);
            _north = GetAdjacentTile(Vector2Int.up);
            _south = GetAdjacentTile(Vector2Int.down);

            HookBreakEvent(_east, _west, _north, _south);
            FixSprite();
        }

        private void FixSprite()
        {
            var result =
                GetVectorResult(Vector2Int.right, _east) +
                GetVectorResult(Vector2Int.left, _west) +
                GetVectorResult(Vector2Int.up, _north) +
                GetVectorResult(Vector2Int.down, _south) +
                GetVectorResult(Vector2Int.right + Vector2Int.up, _north, _east) +
                GetVectorResult(Vector2Int.left + Vector2Int.up, _north, _west) +
                GetVectorResult(Vector2Int.right + Vector2Int.down, _south, _east) +
                GetVectorResult(Vector2Int.left + Vector2Int.down, _south, _west);

            if (_north != null && !_north.IsBroken && _south != null && !_south.IsBroken)
            {
                result.x = 0;
            }

            if (_east != null && !_east.IsBroken && _west != null && !_west.IsBroken)
            {
                result.y = 0;
            }

            SpriteRenderer.sprite = GetCorrectSprite(result);
        }

        private void ClearBreakEvent()
        {
            if (_east != null) _east.OnBreak -= Tile_OnBreak;
            if (_west != null) _west.OnBreak -= Tile_OnBreak;
            if (_north != null) _north.OnBreak -= Tile_OnBreak;
            if (_south != null) _south.OnBreak -= Tile_OnBreak;
        }

        private void HookBreakEvent(params BaseTile[] tiles)
        {
            foreach (var tile in tiles)
            {
                if (tile == null) continue;
                tile.OnBreak += Tile_OnBreak;
            }
        }

        private void Tile_OnBreak()
        {
            FixSprite();
        }

        private Vector2Int GetVectorResult(Vector2Int direction, params BaseTile[] tiles)
        {
            var res = true;
            foreach (var tile in tiles)
            {
                if (tile != null && !tile.IsBroken) continue;
                res = false;
                break;
            }
            return res ? -direction : Vector2Int.zero;
        }

        private Sprite GetCorrectSprite(Vector2Int direction)
        {
            direction.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

            if (direction == Vector2Int.zero) return CenterSprite;
            if (direction == Vector2Int.right) return EastSprite;
            if (direction == Vector2Int.left) return WestSprite;
            if (direction == Vector2Int.up) return NorthSprite;
            if (direction == Vector2Int.down) return SouthSprite;
            if (direction == Vector2Int.right + Vector2Int.up) return NorthEastSprite;
            if (direction == Vector2Int.left + Vector2Int.up) return NorthWestSprite;
            if (direction == Vector2Int.right + Vector2Int.down) return SouthEastSprite;
            return direction == Vector2Int.left + Vector2Int.down ? SouthWestSprite : CenterSprite;
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