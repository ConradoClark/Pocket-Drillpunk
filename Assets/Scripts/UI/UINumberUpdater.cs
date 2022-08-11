using System.Collections.Generic;
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
            Counter_OnChange(new Counter.CounterChangeArgs
            {
                OldCount = 0,
                NewCount= Counter.Count
            });
        }

        private void OnEnable()
        {
            Counters[Counter] = this;
            NumberRenderer.Number = Counter.Count;
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
