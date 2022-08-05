using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UINumberFlipAnimation : UINumberAnimation
    {
        public float AnimationSpeed = 1;

        private int[] _numbersToFlip;

        public override IEnumerable<IEnumerable<Action>> AnimateOldNumbers(UINumberRenderer numberRenderer, int currentNumber, int newNumber)
        {
            var currentNumberStr = numberRenderer.GetStringValue(currentNumber);
            var newNumberStr = numberRenderer.GetStringValue(newNumber);

            if (currentNumberStr.Length != newNumberStr.Length) yield break;

            _numbersToFlip = currentNumberStr.SelectMany((n, ix) => newNumberStr[ix] == n ? Array.Empty<int>() : new[] { ix }).ToArray();

            foreach (var n in _numbersToFlip)
            {
                numberRenderer.Renderers[n].transform.localScale = new Vector3(1, 0.5f, 1);
            }

            if (AnimationSpeed <= 0) AnimationSpeed = 1;

            yield return TimeYields.WaitMilliseconds(UITimer, 100 / AnimationSpeed);

            foreach (var n in _numbersToFlip)
            {
                numberRenderer.Renderers[n].enabled = false;
            }

            yield return TimeYields.WaitMilliseconds(UITimer, 100 / AnimationSpeed);
        }

        public override IEnumerable<IEnumerable<Action>> AnimateNewNumbers(UINumberRenderer numberRenderer)
        {
            foreach (var n in _numbersToFlip)
            {
                numberRenderer.Renderers[n].enabled = true;
            }
            yield return TimeYields.WaitMilliseconds(UITimer, 100 / AnimationSpeed);

            foreach (var n in _numbersToFlip)
            {
                numberRenderer.Renderers[n].transform.localScale = Vector3.one;
            }
        }
    }
}
