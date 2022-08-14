using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    [CreateAssetMenu(fileName = "DrillSkill", menuName = "Drillpunk/Battle/DrillSkill", order = 1)]
    public class DrillSkill : ScriptableObject
    {
        public ScriptPrefab SkillEffect;
        public float EffectDelayInSeconds;

        public BattleElement Element;
        public int Power;
        public int Shield;
        public bool Stun;
        public int SelfDamage;
        public string Name;

        public int NeutralCost;
        public int FireCost;
        public int IceCost;
        public int BioCost;
        public int CrystalCost;

        public bool ScalesWithDrillPower;
        public bool ScalesWithJetpackPower;
        public bool ScalesWithMaxHP;

        public string Animation;
        public float DurationInSeconds;

        public int CalculateDamage(int drillPower, int jetpackPower, int maxHP, BattleElement enemyElement)
        {
            return Power +
                   (ScalesWithDrillPower ? (drillPower - 1) : 0) +
                   (ScalesWithJetpackPower ? (jetpackPower - 1) : 0) +
                   (ScalesWithMaxHP ? (maxHP - 4) : 0);
        }

        public int CalculateShield(int drillPower, int jetpackPower, int maxHP)
        {
            return Shield +
                   (ScalesWithDrillPower ? (drillPower - 1) : 0) +
                   (ScalesWithJetpackPower ? (jetpackPower - 1) : 0) +
                   (ScalesWithMaxHP ? (maxHP - 4) : 0);
        }
    }
}
