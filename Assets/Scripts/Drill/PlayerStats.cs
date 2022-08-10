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
        public int HP;
        public int MaxHP;
        public int DrillPower;
        public int JetpackBattery;

        public Counter HPCounter;
        public Counter MaxHPCounter;

        public List<DrillSkill> Moves;

        private void Awake()
        {
            HP = MaxHP = HPCounter.Count = MaxHPCounter.Count = 3;
        }

        public void SetHP(int value)
        {
            if (value <= 0) value = 0;
            HP = HPCounter.Count = Math.Max(value, MaxHP);
        }

        public void SetMaxHP(int value)
        {
            MaxHP = MaxHPCounter.Count =  value;
        }
    }
}
