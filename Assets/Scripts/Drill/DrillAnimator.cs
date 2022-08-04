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

    public DrillingController DrillController;
    public Animator Animator;
    public SpriteRenderer SpriteRenderer;

    // Smoke Burst
    [Header("Smoke Burst")]
    public ScriptPrefab SmokeBurst;
    public ScriptPrefab DrillingSmokeBurst;
    public Transform SmokeBurstPositionReference;
    public float SmokeBurstFrequencyStandingInMs;
    public float SmokeBurstFrequencyWalkingInMs;
    public float SmokeBurstFrequencyDrillingInMs;
    public Vector2 SmokeBurstOffset;

    // Permanent Smoke
    [Header("Perma Smoke")]
    public SpriteRenderer PermanentSmoke;
    public Transform PermanentSmokePositionReference;
    public Animator PermanentSmokeAnimator;

    // JetpackSmoke
    [Header("Jetpack")]
    public Vector3 JetpackSmokeOffset;
    public SpriteRenderer JetpackSmoke;

    // Drill Impact
    [Header("Drill Impact")] public ScriptPrefab DrillImpactBurst;

    private bool _enabled;
    private bool _isMoving;
    private bool _facingRight;
    private Vector3 _jetpackSmokeLocalPosition;

    private void OnEnable()
    {
        _enabled = true;
        _facingRight = true;
        _jetpackSmokeLocalPosition = JetpackSmoke.transform.localPosition;
        CharacterController.OnTurn += CharacterController_OnTurn;
        CharacterController.OnStartMoving += CharacterController_OnStartMoving;
        CharacterController.OnStopMoving += CharacterController_OnStopMoving;

        DrillController.OnStartDrilling += DrillController_OnStartDrilling;
        DrillController.OnStopDrilling += DrillController_OnStopDrilling;
        DrillController.OnDrillImpact += DrillController_OnDrillImpact;

        DefaultMachinery.AddBasicMachine(HandleSmoke());
    }

    private void DrillController_OnDrillImpact(Vector2Int obj)
    {
        if (DrillImpactBurst.Pool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = transform.position;
        }
    }

    private void DrillController_OnStopDrilling(Vector2Int obj)
    {
        PermanentSmokeAnimator.SetBool("Drilling", false);
        Animator.SetBool("Drilling", false);
    }

    private void DrillController_OnStartDrilling(Vector2Int obj)
    {
        PermanentSmokeAnimator.SetBool("Drilling", true);
        Animator.SetBool("Drilling", true);
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

        JetpackSmoke.transform.localPosition =
            _jetpackSmokeLocalPosition + new Vector3(JetpackSmokeOffset.x * (SpriteRenderer.flipX ? -1 : 1), JetpackSmokeOffset.y);
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
            var burst = DrillController.IsDrilling ? DrillingSmokeBurst : SmokeBurst;
            if (burst.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = new Vector3(
                    transform.position.x + (_facingRight ? 1 : -1) * SmokeBurstPositionReference.transform.localPosition.x + SmokeBurstOffset.x,
                    transform.position.y + SmokeBurstPositionReference.transform.localPosition.y + SmokeBurstOffset.y, 0);

                effect.Component.GetComponent<SpriteRenderer>().flipX = !_facingRight;
            }

            yield return TimeYields.WaitMilliseconds(GameTimer, GetSmokeBurstFrequency());
        }
    }

    private float GetSmokeBurstFrequency()
    {
        return DrillController.IsDrilling ? SmokeBurstFrequencyDrillingInMs :
            _isMoving ? SmokeBurstFrequencyWalkingInMs : SmokeBurstFrequencyStandingInMs;
    }


    private void OnDisable()
    {
        _enabled = false;
        CharacterController.OnTurn -= CharacterController_OnTurn;
        CharacterController.OnStartMoving -= CharacterController_OnStartMoving;
        CharacterController.OnStopMoving -= CharacterController_OnStopMoving;

        DrillController.OnStartDrilling -= DrillController_OnStartDrilling;
        DrillController.OnStopDrilling -= DrillController_OnStopDrilling;
        DrillController.OnDrillImpact -= DrillController_OnDrillImpact;
    }
}
