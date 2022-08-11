using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class ResetCounters : MonoBehaviour
    {
        public Counter[] Counters;

        private void OnEnable()
        {
            foreach (var counter in Counters)
            {
                counter.Count = 0;
            }
        }
    }
}
