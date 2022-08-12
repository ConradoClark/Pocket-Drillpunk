using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Drill;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using UnityEngine;

public class Player : MonoBehaviour
{
    public DrillingController DrillController;
    public DrillCharacterController CharacterController;
    public LichtPlatformerJumpController JumpController;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerStats = SceneObject<PlayerStats>.Instance();
    }

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

    public void LevelUp(LevelUpOption option)
    {
        _playerStats.LevelUp(option);
    }
}