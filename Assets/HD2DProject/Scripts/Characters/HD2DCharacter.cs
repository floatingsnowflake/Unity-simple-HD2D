using System;
using TeamMingo.Characters.Runtime;
using TeamMingo.Input.Runtime;
using UnityEngine;

namespace HD2DProject.Scripts.Characters
{
  public class HD2DCharacter : CharacterBase
  {
    
    public float speed = 5.0f;

    private Transform _transform;
    private InputDirection _direction = InputDirection.Down;

    private void Awake()
    {
      _transform = transform;
    }

    private void Update()
    {
      var movement = input.Movement * -1;
      var isZeroMovement = movement == Vector2.zero;
      var dir = InputDirectionExtensions.Parse(movement);
      if (dir == InputDirection.None)
      {
        dir = _direction;
      }
      _direction = dir;
      
      switch (dir)
      {
        case InputDirection.Down:
          Entity.Play(isZeroMovement ? "StandUp" : "WalkUp");
          break;
        case InputDirection.Up:
          Entity.Play(isZeroMovement ? "StandDown" : "WalkDown");
          break;
        case InputDirection.Left:
        case InputDirection.LeftDown:
        case InputDirection.LeftUp:
          Entity.Play(isZeroMovement ? "StandLeft" : "WalkLeft");
          break;
        case InputDirection.Right:
        case InputDirection.RightDown:
        case InputDirection.RightUp:
          Entity.Play(isZeroMovement ? "StandRight" : "WalkRight");
          break;
        default: 
          Entity.Play(isZeroMovement ? "StandDown" : "WalkDown");
          break;
      }
      var delta = (movement * (speed * Time.deltaTime));
      ApplyMoveDelta(new Vector3(delta.x, 0, delta.y));
    }

    protected virtual void ApplyMoveDelta(Vector3 delta)
    {
      _transform.position += delta;
    }
  }
}