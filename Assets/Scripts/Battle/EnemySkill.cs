using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    [CreateAssetMenu(fileName = "DrillSkill", menuName = "Drillpunk/Battle/DrillSkill", order = 1)]
    public class EnemySkill : ScriptableObject
    {
        public ScriptPrefab SkillEffect;
        public float EffectDelayInSeconds;

        public BattleElement Element;
        public int Power;
        public int Shield;
        public string Name;

        public string Animation;
        public float DurationInSeconds;
    }
}
