using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
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
    private Grid _grid;
    private LichtPhysics _physics;

    public event Action<Vector2Int> OnStartDrilling;
    public event Action<Vector2Int> OnStopDrilling;
    public event Action<Vector2Int> OnDrillImpact;

    protected override void OnAwake()
    {
        base.OnAwake();
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        _drillAction = playerInput.actions[DrillInput.ActionName];
        _grid = SceneObject<Grid>.Instance();
        _physics = this.GetLichtPhysics();
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
            if (_drillAction.IsPressed() && !IsBlocked)
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

                while (_drillAction.IsPressed() && !IsBlocked)
                {
                    direction = CharacterController.CurrentDirection;
                    var drillDirection = DrillDirections.FirstOrDefault(d => d.Direction == direction);

                    Collider2D tileCollider;
                    if (CharacterController.PhysicsObject.GetPhysicsTriggerWithSource(drillDirection.CollisionTrigger,
                            out var source) && source is LichtPhysicsCollisionDetector collisionDetector &&
                        (tileCollider = collisionDetector.Triggers
                            .OrderBy(t=>t.Collider?.Distance(collisionDetector.Collider).distance ?? 999)
                            .FirstOrDefault(t => t.TriggeredHit).Collider) != null &&
                        _physics.TryGetPhysicsObjectByCollider(tileCollider, out var targetTile) &&
                        targetTile.TryGetCustomObject(out BaseTile tile))
                    {

                        tile.Hit(1, direction);
                        OnDrillImpact?.Invoke(direction);

                        yield return TimeYields.WaitMilliseconds(GameTimer, DrillImpactInMs);
                    }

                    yield return TimeYields.WaitOneFrameX;
                }

                IsDrilling = false;
                CharacterController.RemoveMovementMultipliers(this, dampeners);
                OnStopDrilling?.Invoke(CharacterController.CurrentDirection);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
