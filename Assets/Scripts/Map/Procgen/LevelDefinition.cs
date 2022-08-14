using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Drillpunk/ProcGen/LevelDefinition", order = 1)]
public class LevelDefinition : ScriptableObject
{
    public MapRules.WeightRule[] Rules;
    public Color BackgroundColor;
    public Color GlobalLightColor;

    public void CopyFrom(LevelDefinition definition)
    {
        Rules = new MapRules.WeightRule[definition.Rules.Length];
        definition.Rules.CopyTo(Rules,0);
    }
}
