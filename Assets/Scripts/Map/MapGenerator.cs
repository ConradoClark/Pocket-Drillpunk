using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class MapGenerator : BaseGameObject
{
    public ProcGenMapReference MainReference;
    public MapRules[] GlobalRules;

    private Player _player;
    private Grid _grid;
    private Vector3Int _referencePosition;

    public event Action OnPopulate;
    public MapRules RulesToManipulate;
    public MapRules BossRule;
    public LevelDefinition CurrentLevelDefinition;
    public Light2D GlobalLight;

    private string _seed;

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
        _grid = SceneObject<Grid>.Instance();
    }
    private void OnEnable()
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
        var charAmount = Random.Range(10, 20); //set those to the minimum and maximum length of your string
        var sb = new StringBuilder();
        for (var i = 0; i < charAmount; i++)
        {
            sb.Append(glyphs[Random.Range(0, glyphs.Length)]);
        }

        _seed = sb.ToString();
        DefaultMachinery.AddBasicMachine(Initialize());
    }

    private void ManipulateRules()
    {
        Camera.main.backgroundColor = CurrentLevelDefinition.BackgroundColor;
        GlobalLight.color = CurrentLevelDefinition.GlobalLightColor;
        BossRule.MaxDepthLimit = CurrentLevelDefinition.BossDepth;
        var rulesToAdd = new List<MapRules.WeightRule>();
        foreach (var rule in CurrentLevelDefinition.Rules)
        {
            var matched = false;
            var matchedRuleIndex = 0;
            for (var i = 0; i < RulesToManipulate.WeightRules.Length; i++)
            {
                if (RulesToManipulate.WeightRules[i].TileIdentifier != rule.TileIdentifier) continue;
                matched = true;
                matchedRuleIndex = i;
                break;
            }

            if (matched)
            {
                RulesToManipulate.WeightRules[matchedRuleIndex] = rule;
            }
            else
            {
                rulesToAdd.Add(rule);
            }
        }

        RulesToManipulate.WeightRules = RulesToManipulate.WeightRules.Concat(rulesToAdd).ToArray();
    }

    private IEnumerable<IEnumerable<Action>> Initialize()
    {
        ManipulateRules();

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
        MainReference.Populate(_seed, pos);
    }
}
