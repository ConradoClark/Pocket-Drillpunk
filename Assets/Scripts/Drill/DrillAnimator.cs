using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillAnimator : MonoBehaviour
{
    public DrillCharacterController CharacterController;
    public Animator Animator;

    private void OnEnable()
    {
        CharacterController.OnTurn += CharacterController_OnTurn;
        CharacterController.OnStartMoving += CharacterController_OnStartMoving;
        CharacterController.OnStopMoving += CharacterController_OnStopMoving;
    }

    private void CharacterController_OnStopMoving(Vector2Int obj)
    {
        Animator.SetBool("IsMoving", false);
    }

    private void CharacterController_OnStartMoving(Vector2Int obj)
    {
        Animator.SetBool("IsMoving", true);
    }

    private void CharacterController_OnTurn(DrillCharacterController.DirectionEventArgs obj)
    {
        Animator.SetTrigger("Turn");
        if (obj.CurrentDirection != Vector2Int.zero)
        {
            Animator.SetBool("FacingRight", obj.CurrentDirection.x > 0);
        }
    }

    private void OnDisable()
    {
        CharacterController.OnTurn -= CharacterController_OnTurn;
        CharacterController.OnStartMoving -= CharacterController_OnStartMoving;
        CharacterController.OnStopMoving -= CharacterController_OnStopMoving;
    }
}
