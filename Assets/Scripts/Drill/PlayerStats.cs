using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Battle;
using Assets.Scripts.Inventory;
using UnityEngine;

namespace Assets.Scripts.Drill
{
    public class PlayerStats : MonoBehaviour
    {
        public int MaxHP;
        public int DrillPower;
        public int JetpackBattery;

        public int Level => LevelCounter.Count;
        public Counter LevelCounter;
        public Counter HPCounter;
        public Counter MaxHPCounter;

        public List<DrillSkill> Moves;

        public Counter ToNextLevel;

        [Serializable]
        public struct LearnableMove
        {
            public int Level;
            public DrillSkill Skill;
        }

        public LearnableMove[] LearnableMoves;

        private void Start()
        {
            DrillPower = JetpackBattery = 1;
            MaxHP = HPCounter.Count = MaxHPCounter.Count = 3;
            LevelCounter.Count = 1;
            ToNextLevel.Count = GetNextLevelExp();
        }

        private int GetNextLevelExp()
        {
            return (int)Math.Pow(Level + 1, 2) + 5 + (Level - 1) * 3;
        }

        public void IncreaseHP(int value)
        {
            SetHP(HPCounter.Count + 1);
        }

        public void SetHP(int value)
        {
            if (value <= 0) value = 0;
            HPCounter.Count = Math.Min(value, MaxHP);
        }

        public void LevelUp(LevelUpOption option)
        {
            LevelCounter.Count++;
            ToNextLevel.Count = GetNextLevelExp();

            switch (option)
            {
                case LevelUpOption.DrillPower:
                    DrillPower++;
                    break;
                case LevelUpOption.JetpackFuel:
                    JetpackBattery++;
                    break;
                case LevelUpOption.HealthUp:
                    MaxHP++;
                    HPCounter.Count = MaxHPCounter.Count = MaxHP;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }

            foreach (var learnable in LearnableMoves.Where(l => l.Level == Level))
            {
                Moves.Add(learnable.Skill);
            }
        }
    }
}
