using Assets.Scripts.Map;
using UnityEngine;

public class RandomAnimatorController : MonoBehaviour
{
    public SurfaceProp Prop;
    public Animator Animator;
    public RuntimeAnimatorController[] Controllers;

    private void OnEnable()
    {
        if (Prop == null)
        {
            if (Controllers.Length > 0)
            {
                Animator.runtimeAnimatorController = Controllers[Random.Range(0, Controllers.Length)];
            }
            return;
        }
        Prop.OnLoadState += Prop_OnLoadState;

    }

    private void OnDisable()
    {
        if (Prop == null) return;

        Prop.OnLoadState -= Prop_OnLoadState;
    }

    private void Prop_OnLoadState()
    {
        if (Controllers.Length == 0) return;
        var rng = new System.Random($"{Prop.Tile.GeneratedBySeed}_{GetInstanceID()}".GetHashCode());
        Animator.runtimeAnimatorController = Controllers[rng.Next(0, Controllers.Length)];
    }


}
