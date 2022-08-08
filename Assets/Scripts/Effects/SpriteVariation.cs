using UnityEngine;
using Random = System.Random;

[DefaultExecutionOrder(9999)]
public class SpriteVariation : MonoBehaviour
{
    public BaseTile BaseTile;
    public Sprite[] Sprites;
    private SpriteRenderer _spriteRenderer;
    private bool _executed;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (BaseTile == null)
        {

            return;
        }

        BaseTile.OnSeedChanged += BaseTile_OnSeedChanged;
    }

    private void BaseTile_OnSeedChanged(string obj)
    {
        _spriteRenderer.sprite = Sprites[new Random(BaseTile.GeneratedBySeed.GetHashCode()).Next(0, Sprites.Length)];
    }

    private void OnDisable()
    {
        if (BaseTile!=null) BaseTile.OnSeedChanged -= BaseTile_OnSeedChanged;
    }
}
