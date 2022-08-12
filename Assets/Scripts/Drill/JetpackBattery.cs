using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Drill
{
    public class JetpackBattery : BaseUIObject
    {
        public float MinimumSize;
        public float MaximumSize;
        public SpriteRenderer BatteryCounter;

        public int UnscaledSize = 1000;
        private int _currentValue;

        public bool HasBattery => _currentValue > 0;

        private PlayerStats _playerStats;

        protected override void OnAwake()
        {
            base.OnAwake();
            _playerStats = SceneObject<PlayerStats>.Instance(true);
            _currentValue = UnscaledSize;
        }

        public void Refill(int value)
        {
            _currentValue += value;
            if (_currentValue > UnscaledSize) _currentValue = UnscaledSize;
            UpdateSize();
        }

        public bool SpendBattery()
        {
            if (_currentValue == 0) return false;
            var performance = 1 + (_playerStats.JetpackBattery - 1) * 0.5f;
            var amountToSpend = (int)(10 / performance);

            _currentValue -= amountToSpend;
            if (_currentValue <= 0) _currentValue = 0;
            UpdateSize();
            return true;
        }

        private void UpdateSize()
        {
            BatteryCounter.size = new Vector2(MinimumSize + MaximumSize * ((float)_currentValue / UnscaledSize), BatteryCounter.size.y);
        }

    }
}
