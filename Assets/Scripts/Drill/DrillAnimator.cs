using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class DrillAnimator : BaseGameObject
{
    // Basic 
    [Header("Basic")]
    public DrillCharacterController CharacterController;
    public Animator Animator;
    public SpriteRenderer SpriteRenderer;

    // Smoke Burst
    [Header("Smoke Burst")]
    public ScriptPrefab SmokeBurst;
    public Transform SmokeBurstPositionReference;
    public float SmokeBurstFrequencyStandingInMs;
    public float SmokeBurstFrequencyWalkingInMs;
    public float SmokeBurstFrequencyDrillingInMs;
    public Vector2 SmokeBurstOffset;

    public SpriteRenderer PermanentSmoke;
    public Transform PermanentSmokePositionReference;
    
    private bool _enabled;
    private bool _isMoving;
    private bool _facingRight;

    private void OnEnable()
    {
        _enabled = true;
        _facingRight = true;
        CharacterController.OnTurn += CharacterController_OnTurn;
        CharacterController.OnStartMoving += CharacterController_OnStartMoving;
        CharacterController.OnStopMoving += CharacterController_OnStopMoving;
        DefaultMachinery.AddBasicMachine(HandleSmoke());
    }

    private void Update()
    {
        PermanentSmoke.flipX = SpriteRenderer.flipX;
        PermanentSmoke.transform.position = transform.position +
                                            new Vector3((SpriteRenderer.flipX ? -1 : 1) * PermanentSmokePositionReference.localPosition.x,
                                                PermanentSmokePositionReference.localPosition.y);
        PermanentSmoke.transform.rotation = transform.rotation * PermanentSmokePositionReference.rotation;
        if (SpriteRenderer.flipX)
        {
            PermanentSmoke.transform.rotation = Quaternion.Inverse(PermanentSmoke.transform.rotation);
        }
    }

    private void CharacterController_OnStopMoving(Vector2Int obj)
    {
        _isMoving = false;
        Animator.SetBool("IsMoving", false);
    }

    private void CharacterController_OnStartMoving(Vector2Int obj)
    {
        _isMoving = true;
        Animator.SetBool("IsMoving", true);
    }

    private void CharacterController_OnTurn(DrillCharacterController.DirectionEventArgs obj)
    {
        if (obj.CurrentDirection.x != 0)
        {
            Animator.SetTrigger("Turn");

            if (obj.CurrentDirection != Vector2Int.zero)
            {
                _facingRight = obj.CurrentDirection.x > 0;
                Animator.SetBool("FacingRight", _facingRight);
            }

            return;
        }

        // turning down
        if (obj.CurrentDirection.y == -1)
        {
            Animator.SetTrigger("TurnDown");
        }

        // turning up
        if (obj.CurrentDirection.y == 1)
        {
            Animator.SetTrigger("TurnUp");
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleSmoke()
    {
        while (_enabled)
        {
            if (SmokeBurst.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = new Vector3(
                    transform.position.x + (_facingRight ? 1 : -1) * SmokeBurstPositionReference.transform.localPosition.x + SmokeBurstOffset.x, 
                    transform.position.y + SmokeBurstPositionReference.transform.localPosition.y + SmokeBurstOffset.y, 0);

                effect.Component.GetComponent<SpriteRenderer>().flipX = !_facingRight;
            }

            yield return TimeYields.WaitMilliseconds(GameTimer,
                _isMoving ? SmokeBurstFrequencyWalkingInMs : SmokeBurstFrequencyStandingInMs);
        }
    }


    private void OnDisable()
    {
        _enabled = false;
        CharacterController.OnTurn -= CharacterController_OnTurn;
        CharacterController.OnStartMoving -= CharacterController_OnStartMoving;
        CharacterController.OnStopMoving -= CharacterController_OnStopMoving;
    }
}
