using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Map;
using Licht.Interfaces.Generation;
using Licht.Unity.Objects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public abstract class ProcGenMapReference : BaseGameObject, IWeighted<float>, IGenerator<int, float>
{
    public Color GizmoColor;
    public virtual Vector2Int RefSize { get; }
    protected List<Vector2Int> PopulatedPositions;

    protected Grid Grid;
    protected MapGenerator MapGenerator;
    protected GameTilemap GameTilemap;
    protected override void OnAwake()
    {
        base.OnAwake();
        Grid = SceneObject<Grid>.Instance();
        PopulatedPositions = new List<Vector2Int>();
        MapGenerator = SceneObject<MapGenerator>.Instance();
        GameTilemap = SceneObject<GameTilemap>.Instance();
    }

    // Uses TopLeft as Pivot
    public abstract void Populate(string seed, Vector2Int position);

    private void OnDrawGizmosSelected()
    {
        if (PopulatedPositions == null) return;
        foreach (var pos in PopulatedPositions)
        {
            var worldPos = Grid.CellToWorld((Vector3Int)pos);
            Gizmos.color = GizmoColor;
            var size = Grid.cellSize.x * (Vector2)RefSize;

            Gizmos.DrawLine(worldPos - new Vector3(0, 0), worldPos + new Vector3(size.x, 0));
            Gizmos.DrawLine(worldPos - new Vector3(0, 0), worldPos + new Vector3(0,  size.y));
            Gizmos.DrawLine(worldPos + new Vector3(0, size.y), worldPos + new Vector3(size.x, size.y));
            Gizmos.DrawLine(worldPos + new Vector3(size.x, 0), worldPos + new Vector3(size.x, size.y));
        }

    }

    public float Weight { get; set; }
    public int Seed { get; set; }

    public float Generate()
    {
        return (float) new Random(Seed).NextDouble();
    }
}
