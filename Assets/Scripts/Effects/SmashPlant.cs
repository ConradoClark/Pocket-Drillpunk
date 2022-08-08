using System.Collections.Generic;
using Assets.Scripts.Map;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class SmashPlant : EffectPoolable, ITileStateExtension
{
    public float MinDistance;
    public Animator Animator;
    private Player _player;

    public bool Smashed { get; private set; }
    private void Awake()
    {
        _player = SceneObject<Player>.Instance();
    }
    private void Update()
    {
        if (Smashed || !(Vector2.Distance(_player.transform.position, transform.position) < MinDistance)) return;

        Smashed = true;
        Animator.SetTrigger("PassedBy");
    }

    public override void OnActivation()
    {
    }

    public bool Dirty => Smashed;

    public void LoadState(Dictionary<string, object> dict)
    {
        Animator.ResetTrigger("PassedBy");
        Animator.Play("Idle");
        Smashed = dict.ContainsKey("Smashed") && (bool)dict["Smashed"];
        if (Smashed)
        {
            Animator.SetTrigger("PassedBy");
        }
    }

    public void SaveState(Dictionary<string, object> dict)
    {
        dict["Smashed"] = Smashed;
    }
}
