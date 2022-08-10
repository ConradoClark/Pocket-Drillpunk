using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    [CreateAssetMenu(fileName = "Counter", menuName = "Drillpunk/UI/Counter", order = 1)]
    public class Counter : ScriptableObject
    {
        public struct CounterChangeArgs
        {
            public int OldCount;
            public int NewCount;
        }

        public event Action<CounterChangeArgs> OnChange;

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                OnChange?.Invoke(new CounterChangeArgs()
                {
                    OldCount = _count,
                    NewCount = value
                });
                _count = value;
            }
        }

        private void Awake()
        {
            if (Count == 0)
            {
                OnChange?.Invoke(new CounterChangeArgs()
                {
                    OldCount = _count,
                    NewCount = 0
                });
            }
        }
    }
}
