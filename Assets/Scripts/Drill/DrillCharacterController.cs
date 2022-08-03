using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DrillCharacterController : LichtMovementController
{
    [Header("Physics")]
    public ScriptIdentifier GroundedIdentifier;
    public LichtPlatformerJumpController JumpController;
    public LichtPhysicsObject PhysicsObject;

    [Header("Movement")]
    public float MaxSpeed;
    public float AccelerationTime;
    public float DecelerationTime;
    public EasingYields.EasingFunction AccelerationEasing;
    public EasingYields.EasingFunction DecelerationEasing;

    [Header("Input")]
    public ScriptInput MoveInput;
    public ScriptInput JumpInput;
    public Vector2Int CurrentDirection { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsHovering { get; private set; }

    [Header("Hover")]
    public float HoverSpeed;
    public float HoverParticlesFrequencyInMs;
    public Vector3 HoverParticlesOffset;
    public ScriptPrefab HoverParticles;
    public SpriteRenderer HoverSmoke;

    public struct DirectionEventArgs
    {
        public Vector2Int PreviousDirection;
        public Vector2Int CurrentDirection;
    }

    public event Action<DirectionEventArgs> OnTurn;
    public event Action<Vector2Int> OnStartMoving;
    public event Action<Vector2Int> OnStopMoving;

    private bool _enabled;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private LichtPhysics _physics;

    protected override void OnAwake()
    {
        base.OnAwake();
        CurrentDirection = Vector2Int.right;
        var playerInput = SceneObject<PlayerInput>.Instance();
        _moveAction = playerInput.actions[MoveInput.ActionName];
        _jumpAction = playerInput.actions[JumpInput.ActionName];
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        _enabled = true;
        DefaultMachinery.AddBasicMachine(HandleInput());
        DefaultMachinery.AddBasicMachine(HandleHover());
    }
    private void OnDisable()
    {
        _enabled = false;
    }

    private bool IsTriggeringHover()
    {
        return !JumpController.IsJumping && _jumpAction.IsPressed() &&
               !PhysicsObject.GetPhysicsTrigger(GroundedIdentifier);
    }

    private IEnumerable<IEnumerable<Action>> SpawnHoverParticles()
    {
        HoverSmoke.enabled = true;
        while (IsHovering)
        {
            yield return TimeYields.WaitMilliseconds(GameTimer, HoverParticlesFrequencyInMs);

            if (HoverParticles.Pool.TryGetFromPool(out var effect))
            {
                var offset = new Vector3(HoverParticlesOffset.x * CurrentDirection.x, HoverParticlesOffset.y);
                effect.Component.transform.SetPositionAndRotation(transform.position + offset,
                    Quaternion.AngleAxis(Random.Range(0, 60f), Vector3.up));
            }
        }

        HoverSmoke.enabled = false;
    }

    private IEnumerable<IEnumerable<Action>> HandleHover()
    {
        while (_enabled)
        {
            if (IsTriggeringHover())
            {
                _physics.BlockCustomPhysicsForceForObject(this, PhysicsObject, JumpController.GravityIdentifier.Name);
                IsHovering = true;
                DefaultMachinery.AddBasicMachine(SpawnHoverParticles());
                do
                {
                    PhysicsObject.ApplySpeed(new Vector2(0, HoverSpeed));
                    yield return TimeYields.WaitOneFrameX;
                } while (IsTriggeringHover());

                _physics.UnblockCustomPhysicsForceForObject(this, PhysicsObject, JumpController.GravityIdentifier.Name);
                IsHovering = false;
                continue;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleInput()
    {
        while (_enabled)
        {
            var movement = _moveAction.ReadValue<Vector2>();
            var direction = GetDirection(movement);

            if (direction == Vector2Int.zero && IsMoving)
            {
                IsMoving = false;
                OnStopMoving?.Invoke(CurrentDirection);
            }

            if (direction != CurrentDirection && direction != Vector2.zero)
            {
                var prevDirection = CurrentDirection;
                CurrentDirection = direction;
                OnTurn?.Invoke(new DirectionEventArgs { PreviousDirection = prevDirection, CurrentDirection = direction });
                if (PhysicsObject.GetPhysicsTrigger(GroundedIdentifier))
                {
                    yield return TimeYields.WaitSeconds(GameTimer, DecelerationTime * (PhysicsObject.LatestNonZeroSpeed.magnitude / MaxSpeed));
                }
                else
                {
                    yield return TimeYields.WaitSeconds(GameTimer, 0.01f);
                }
            }

            if (direction == Vector2Int.left || direction == Vector2Int.right)
            {
                yield return Move(direction).AsCoroutine();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> Move(Vector2Int direction)
    {
        var targetSpeed = (Vector2)direction * MaxSpeed;
        var directionChanged = false;
        var newDirection = direction;

        IsMoving = true;
        OnStartMoving?.Invoke(direction);
        yield return PhysicsObject.GetSpeedAccessor()
            .ToSpeed(targetSpeed)
            .Over(AccelerationTime)
            .Easing(AccelerationEasing)
            .BreakIf(() =>
            {
                (directionChanged, newDirection) = CheckDirectionChanged(direction);
                return directionChanged;
            })
            .UsingTimer(GameTimer)
            .Build();

        if (directionChanged)
        {
            if (newDirection == Vector2Int.zero)
            {
                OnStopMoving?.Invoke(direction);
            }
            yield break;
        }

        while (!directionChanged)
        {
            yield return TimeYields.WaitOneFrameX;
            PhysicsObject.ApplySpeed(targetSpeed);
            (directionChanged, newDirection) = CheckDirectionChanged(direction);
        }

        yield return PhysicsObject.GetSpeedAccessor(targetSpeed)
            .ToSpeed(Vector2.zero)
            .Over(DecelerationTime)
            .Easing(DecelerationEasing)
            .UsingTimer(GameTimer)
            .Build();

        if (newDirection != Vector2Int.zero)
        {
            CurrentDirection = newDirection;
            OnTurn?.Invoke(new DirectionEventArgs { PreviousDirection = direction, CurrentDirection = newDirection });
        }
        else
        {
            OnStopMoving?.Invoke(direction);
            IsMoving = false;
        }
    }

    private (bool, Vector2Int) CheckDirectionChanged(Vector2Int targetDir)
    {
        var movement = _moveAction.ReadValue<Vector2>();
        var direction = GetDirection(movement);
        return (direction != targetDir, direction);
    }

    private Vector2Int GetDirection(Vector2 input)
    {
        var horizontalDir = input.x == 0 ? Vector2Int.zero : input.x > 0 ? Vector2Int.right : Vector2Int.left;
        var verticalDir = input.y == 0 ? Vector2Int.zero : input.y > 0 ? Vector2Int.up : Vector2Int.down;

        if ((horizontalDir != Vector2Int.zero && horizontalDir == CurrentDirection) ||
            (verticalDir != Vector2Int.zero && verticalDir == CurrentDirection))
        {
            return CurrentDirection;
        }

        return Mathf.Abs(input.x) >= Mathf.Abs(input.y) ? horizontalDir : verticalDir;
    }
}
