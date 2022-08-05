using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Inventory;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UINumberUpdater : MonoBehaviour
    {
        public static Dictionary<Counter, UINumberUpdater> Counters = new();

        public UINumberRenderer NumberRenderer;
        public Counter Counter;

        private void Awake()
        {
            Counter.Count = 0;
        }

        private void OnEnable()
        {
            Counters[Counter] = this;
            Counter.OnChange += Counter_OnChange;
        }

        private void OnDisable()
        {
            Counters.Remove(Counter);
            Counter.OnChange -= Counter_OnChange;
        }
        private void Counter_OnChange(Counter.CounterChangeArgs obj)
        {
            NumberRenderer.Number = obj.NewCount;
        }
    }
}
