using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Drill;
using Assets.Scripts.Inventory;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : BaseGameObject
{
    public Vector3 CheckpointPosition;
    public DrillingController DrillController;
    public DrillCharacterController CharacterController;
    public LichtPlatformerJumpController JumpController;
    public LichtPhysicsObject PhysicsObject;

    private PlayerStats _playerStats;
    private bool _spinning;

    public SpriteRenderer SpriteRenderer;
    private UIGameOverPopup _gameOverPopup;
    private ResetCounters _resetCounters;
    protected override void OnAwake()
    {
        base.OnAwake();
        _playerStats = SceneObject<PlayerStats>.Instance();
        _gameOverPopup = SceneObject<UIGameOverPopup>.Instance(true);
        _resetCounters = SceneObject<ResetCounters>.Instance(true);
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

    public void GameOver()
    {
        DefaultMachinery.AddBasicMachine(MoveToLastCheckpoint());
    }

    public IEnumerable<IEnumerable<Action>> MoveToLastCheckpoint()
    {
        Block();
        PhysicsObject.enabled = false;
        _spinning = true;

        var sortingOrder = SpriteRenderer.sortingOrder;

        DefaultMachinery.AddBasicMachine(Spin());
        DefaultMachinery.AddBasicMachine(Blink());

        SpriteRenderer.sortingOrder = 999;

        yield return TimeYields.WaitSeconds(GameTimer, 0.5f);

        yield return transform.GetAccessor()
            .Position
            .ToPosition(CheckpointPosition)
            .Over(Vector2.Distance(transform.position, CheckpointPosition) * 4f)
            .UsingTimer(GameTimer)
            .Build();

        _resetCounters.ResetGems();
        _playerStats.IncreaseHP(1);

        yield return _gameOverPopup.Show().AsCoroutine();

        _spinning = false;
        PhysicsObject.enabled = true;
        SpriteRenderer.sortingOrder = sortingOrder;

        Unblock();
    }

    private IEnumerable<IEnumerable<Action>> Blink()
    {
        while (_spinning)
        {
            SpriteRenderer.enabled = !SpriteRenderer.enabled;
            yield return TimeYields.WaitMilliseconds(GameTimer, 50);
        }

        SpriteRenderer.enabled = true;
    }

    private IEnumerable<IEnumerable<Action>> Spin()
    {
        while (_spinning)
        {
            transform.Rotate(Vector3.forward,10);
            yield return TimeYields.WaitOneFrameX;
        }

        transform.rotation = Quaternion.identity;
    }
}