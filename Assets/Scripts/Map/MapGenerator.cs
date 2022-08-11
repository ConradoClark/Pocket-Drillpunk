using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class MapGenerator : BaseGameObject
{
    public ProcGenMapReference MainReference;
    public MapRules[] GlobalRules;

    private Player _player;
    private Grid _grid;
    private Vector3Int _referencePosition;

    public event Action OnPopulate;
    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
        _grid = SceneObject<Grid>.Instance();
    }
    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Initialize());
    }
    private IEnumerable<IEnumerable<Action>> Initialize()
    {
        yield return TimeYields.WaitOneFrameX;
        Populate(new Vector2Int(-9, 0));
        Populate(new Vector2Int(-9, -9));
        Populate(new Vector2Int(9, 0));
        Populate(new Vector2Int(9, -9));
        Populate(new Vector2Int(0, -9));
        Populate(new Vector2Int(0, 9));

        OnPopulate?.Invoke();
    }
    private void Update()
    {
        var playerPos = MapManager.Get9X9Position(_grid.WorldToCell(_player.transform.position));

        if (_referencePosition != playerPos)
        {
            DefaultMachinery.AddBasicMachine(PopulateOnPlayerPos(_referencePosition, playerPos));
            _referencePosition = playerPos;
        }
    }

    private IEnumerable<IEnumerable<Action>> PopulateOnPlayerPos(Vector3Int referencePosition, Vector3Int playerPos)
    {
        var popPos = (Vector2Int)playerPos * 9;
        if (playerPos.x != referencePosition.x)
        {
            if (playerPos.x > referencePosition.x) popPos += new Vector2Int(9, 0);
            if (playerPos.x < referencePosition.x) popPos += new Vector2Int(-9, 0);
            Populate(popPos);
            yield return TimeYields.WaitOneFrameX;
            Populate(popPos - new Vector2Int(0, -9));
            yield return TimeYields.WaitOneFrameX;
            Populate(popPos - new Vector2Int(0, 9));
        }

        if (playerPos.y != referencePosition.y)
        {
            if (playerPos.y > referencePosition.y) popPos += new Vector2Int(0, 9);
            if (playerPos.y < referencePosition.y) popPos += new Vector2Int(0, -9);
            Populate(popPos);
            yield return TimeYields.WaitOneFrameX;
            Populate(popPos - new Vector2Int(-9, 0));
            yield return TimeYields.WaitOneFrameX;
            Populate(popPos - new Vector2Int(9, 0));
        }

        OnPopulate?.Invoke();
    }

    public void Populate(Vector2Int pos)
    {
        MainReference.Populate("XXX3", pos);
    }
}
