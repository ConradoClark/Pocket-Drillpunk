using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrillingController : LichtMovementController
{
    [Serializable]
    public struct DrillColliderDefinition
    {
        public Vector2Int Direction;
        public ScriptIdentifier CollisionTrigger;
    }

    public DrillCharacterController CharacterController;
    public ScriptInput DrillInput;
    public DrillColliderDefinition[] DrillDirections;
    public float MovementDampFactorWhenDrilling;
    public float DrillImpactInMs;
    public bool IsDrilling { get; private set; }

    private InputAction _drillAction;
    private bool _enabled;
    private bool _grid;

    public event Action<Vector2Int> OnStartDrilling;
    public event Action<Vector2Int> OnStopDrilling;

    protected override void OnAwake()
    {
        base.OnAwake();
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        _drillAction = playerInput.actions[DrillInput.ActionName];
        _grid = SceneObject<Grid>.Instance();
    }

    private void OnEnable()
    {
        _enabled = true;
        DefaultMachinery.AddBasicMachine(HandleDrilling());
    }

    private void OnDisable()
    {
        _enabled = false;
    }

    private IEnumerable<IEnumerable<Action>> HandleDrilling()
    {
        while (_enabled)
        {
            if (_drillAction.IsPressed())
            {
                var dampeners = new[]
                {
                    new Vector2(MovementDampFactorWhenDrilling, 0),
                    new Vector2(-MovementDampFactorWhenDrilling, 0),
                };

                IsDrilling = true;
                var direction = CharacterController.CurrentDirection;
                OnStartDrilling?.Invoke(direction);
                CharacterController.SetMovementMultipliers(this, dampeners);

                while (_drillAction.IsPressed())
                {
                    yield return TimeYields.WaitMilliseconds(GameTimer, DrillImpactInMs);
                }

                IsDrilling = false;
                CharacterController.RemoveMovementMultipliers(this, dampeners);
                OnStopDrilling?.Invoke(CharacterController.CurrentDirection);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
