using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Drillpunk/ProcGen/LevelDefinition", order = 1)]
public class LevelDefinition : ScriptableObject
{
    public MapRules.WeightRule[] Rules;
    public Color BackgroundColor;
    public Color GlobalLightColor;
    public ScriptPrefab BossBattler;
    public int BossDepth;

    public void CopyFrom(LevelDefinition definition)
    {
        Rules = new MapRules.WeightRule[definition.Rules.Length];
        BackgroundColor = definition.BackgroundColor;
        GlobalLightColor = definition.GlobalLightColor;
        BossBattler = definition.BossBattler;
        BossDepth = definition.BossDepth;
        definition.Rules.CopyTo(Rules,0);
    }
}
