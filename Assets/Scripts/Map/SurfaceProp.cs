using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Map
{
    public class SurfaceProp : EffectPoolable, ITileStateExtension
    {
        public Vector3 Offset;
        public BaseTile Tile;
        private Grid _grid;
        private ITileStateExtension[] _extensions;

        public event Action OnLoadState;

        private void Awake()
        {
            _grid = SceneObject<Grid>.Instance();
            _extensions = GetComponents<ITileStateExtension>().Except(new[] { this }).ToArray();
        }

        public void SetPositionAndRotationBasedOnSide(Vector2Int side)
        {
            var pos = _grid.cellSize * 0.5f + Offset;

            transform.SetPositionAndRotation(
                Tile.transform.position + (Vector3)Vector2.Scale(pos, side),
                Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, side), Vector3.forward)
                );
        }

        private void OnDisable()
        {
            if (Tile == null) return;
            Tile.OnRelease -= Tile_OnRelease;
        }

        public void HookOnTileRelease()
        {
            Tile.OnRelease += Tile_OnRelease;
        }

        private void Tile_OnRelease()
        {
            EndEffect();
        }

        public bool Dirty => _extensions.Any(e => e.Dirty);

        public void LoadState(Dictionary<string, object> dict)
        {
            OnLoadState?.Invoke();
            foreach (var extension in _extensions)
            {
                extension.LoadState(dict);
            }
        }

        public void SaveState(Dictionary<string, object> dict)
        {
            foreach (var extension in _extensions)
            {
                if (extension.Dirty) extension.SaveState(dict);
            }
        }

        public override void OnActivation()
        {
        }
    }
}
