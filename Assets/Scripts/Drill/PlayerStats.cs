using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Counter HPCounter;
        public Counter MaxHPCounter;

        public List<DrillSkill> Moves;

        public int Level;
        public Counter ToNextLevel;

        private void Start()
        {
            MaxHP = HPCounter.Count = MaxHPCounter.Count = 3;
            Level = 1;
            ToNextLevel.Count = GetNextLevelExp();
        }

        private int GetNextLevelExp()
        {
            return (int) Math.Pow(Level + 1, 2) + 5 + (Level-1) * 3;
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

        public void SetMaxHP(int value)
        {
            MaxHP = MaxHPCounter.Count =  value;
        }
    }
}
