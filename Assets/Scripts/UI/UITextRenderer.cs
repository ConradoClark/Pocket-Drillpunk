using System.Collections.Generic;
using System.Text;
using Assets.Scripts.UI;
using Licht.Unity.Objects;
using UnityEngine;

public class UITextRenderer : BaseUIObject
{
    public UITextFont Font;
    public Material DefaultMaterial;
    public int SortingOrder;
    public string Text;
    public int MaxLetters;
    public float DistanceBetweenLetters;

    public SpriteRenderer[] Renderers { get; private set; }

    private string _currentText;
    private Material _currentMaterial;
    private Dictionary<char, Sprite> _charMap;

    protected override void OnAwake()
    {
        base.OnAwake();
        _currentMaterial = DefaultMaterial;
        var renderers = new List<SpriteRenderer>();
        for (var i = 0; i < MaxLetters; i++)
        {
            var sprRenderer = new GameObject($"{gameObject.name}_Letter_{i}")
            {
                layer = LayerMask.NameToLayer("UI")
            };
            var number = sprRenderer.AddComponent<SpriteRenderer>();
            number.material = DefaultMaterial;
            number.transform.SetParent(transform);
            number.sortingOrder = SortingOrder;
            number.transform.localPosition = new Vector3(i * DistanceBetweenLetters, 0);
            renderers.Add(number);
        }

        Renderers = renderers.ToArray();

        _charMap = new Dictionary<char, Sprite>
        {
            { ' ', null },
            { 'A', Font.LetterA },
            { 'B', Font.LetterB },
            { 'C', Font.LetterC },
            { 'D', Font.LetterD },
            { 'E', Font.LetterE },
            { 'F', Font.LetterF },
            { 'G', Font.LetterG },
            { 'H', Font.LetterH },
            { 'I', Font.LetterI },
            { 'J', Font.LetterJ },
            { 'K', Font.LetterK },
            { 'L', Font.LetterL },
            { 'M', Font.LetterM },
            { 'N', Font.LetterN },
            { 'O', Font.LetterO },
            { 'P', Font.LetterP },
            { 'Q', Font.LetterQ },
            { 'R', Font.LetterR },
            { 'S', Font.LetterS },
            { 'T', Font.LetterT },
            { 'U', Font.LetterU },
            { 'V', Font.LetterV },
            { 'W', Font.LetterW },
            { 'X', Font.LetterX },
            { 'Y', Font.LetterY },
            { 'Z', Font.LetterZ },
            { ':', Font.LetterColon },
            { '!', Font.LetterExclamation },
            { '?', Font.LetterQuestion }
        };
    }

    private void OnEnable()
    {
        UpdateText();
    }

    private void OnDisable()
    {
    }

    public void SetText(string text)
    {
        Text = text;
        UpdateText();
    }

    private void Update()
    {
        if (_currentText != Text)
        {
            UpdateText();
        }

        if (_currentMaterial != DefaultMaterial)
        {
            _currentMaterial = DefaultMaterial;
            foreach (var spr in Renderers)
            {
                spr.material = _currentMaterial;
            }
        }
    }

    private void UpdateText()
    {
        _currentText = Text.ToUpper();
        if (string.IsNullOrWhiteSpace(_currentText))
        {
            foreach (var spr in Renderers)
            {
                spr.enabled = false;
            }
            return;
        }

        for (var i = 0; i < MaxLetters; i++)
        {
            if (i >= _currentText.Length || !_charMap.ContainsKey(_currentText[i]))
            {
                Renderers[i].enabled = false;
                continue;
            }

            var letter = Renderers[i];
            letter.enabled = true;
            letter.sprite = _charMap[_currentText[i]];
        }
    }

}
