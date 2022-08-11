using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.CharacterControllers;
using UnityEngine;

public class Player : MonoBehaviour
{
    public DrillingController DrillController;
    public DrillCharacterController CharacterController;
    public LichtPlatformerJumpController JumpController;

    public void Block()
    {
        DrillController.BlockMovement(this);
        CharacterController.BlockMovement(this);
        JumpController.BlockMovement(this);
    }

    public void Unblock()
    {
        DrillController.UnblockMovement(this);
        CharacterController.UnblockMovement(this);
        JumpController.UnblockMovement(this);
    }
}