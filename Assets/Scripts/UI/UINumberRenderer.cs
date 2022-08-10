using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class UINumberRenderer : BaseUIObject
{
    public UINumberFont Font;
    public Material DefaultMaterial;
    public int SortingOrder;
    public int Number;
    public int Digits;
    public bool AnimateNumberOnChange;
    public UINumberAnimation Animation;
    public float DistanceBetweenNumbers;
    public SpriteRenderer Mockup;

    public SpriteRenderer[] Renderers { get; private set; }

    private int _currentNumber;
    private bool _enabled;
    private Dictionary<char, Sprite> _charMap;

    protected override void OnAwake()
    {
        base.OnAwake();
        if (Mockup != null)
        {
            Mockup.enabled = false;
        }
        var renderers = new List<SpriteRenderer>();
        for (var i = 0; i < Digits; i++)
        {
            var sprRenderer = new GameObject($"{gameObject.name}_Digit_{i}")
            {
                layer = LayerMask.NameToLayer("UI")
            };
            var number = sprRenderer.AddComponent<SpriteRenderer>();
            number.material = DefaultMaterial;
            number.transform.SetParent(transform);
            number.sortingOrder = SortingOrder;
            number.transform.localPosition = new Vector3(i * DistanceBetweenNumbers, 0);
            renderers.Add(number);
        }

        Renderers = renderers.ToArray();

        _charMap = new Dictionary<char, Sprite>
        {
            { '0', Font.Number0 },
            { '1', Font.Number1 },
            { '2', Font.Number2 },
            { '3', Font.Number3 },
            { '4', Font.Number4 },
            { '5', Font.Number5 },
            { '6', Font.Number6 },
            { '7', Font.Number7 },
            { '8', Font.Number8 },
            { '9', Font.Number9 },
        };
    }

    private void OnEnable()
    {
        _currentNumber = Number;
        _enabled = true;
        RenderNumber();

        DefaultMachinery.AddBasicMachine(UpdateNumbers());
    }

    private void OnDisable()
    {
        _enabled = false;
    }

    private IEnumerable<IEnumerable<Action>> UpdateNumbers()
    {
        while (_enabled)
        {
            if (_currentNumber != Number)
            {
                if (AnimateNumberOnChange) yield return Animation.AnimateOldNumbers(this, _currentNumber, Number).AsCoroutine();
                _currentNumber = Number;
                RenderNumber();
                if (AnimateNumberOnChange) yield return Animation.AnimateNewNumbers(this).AsCoroutine();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    public string GetStringValue(int number)
    {
        return number.ToString().PadLeft(Digits, '0');
    }

    private void RenderNumber()
    {
        var value = GetStringValue(Math.Clamp(_currentNumber, 0, _currentNumber));

        for (var i = 0; i < Digits; i++)
        {
            Renderers[i].sprite = _charMap[value[i]];
        }
    }

}
