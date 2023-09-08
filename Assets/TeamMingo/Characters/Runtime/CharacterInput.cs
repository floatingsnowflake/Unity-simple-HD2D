using System.Collections.Generic;
using System.Linq;
using TeamMingo.Common.Runtime;
using TeamMingo.Input.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace TeamMingo.Characters.Runtime
{
  public class CharacterInput : MonoBehaviour
  {
    public bool debug;
    private Log LOG;
    public Vector2 Input { get; private set; }
    public InputDirection InputDirection { get; private set; }
    public InputDirection MovementDirection { get; private set; }
    
    public Vector2 Movement { get; private set; }
    public Vector2 Facing { get; private set; } = Vector2.right;
    public bool facingLocked;
    public bool InputDisabled => _inputDisableCounter > 0;
    private int _inputDisableCounter = 0;
    
    public UnityEvent<InputActionData> onInputActionAnyway;
    public UnityEvent<InputActionData> onInputAction;
    public UnityEvent<Vector2> onInputMovement;
    
    private readonly LinkedList<InputActionData> _inputHistory = new();
    private readonly Dictionary<string, bool> _pressingDict = new();

    private void Awake()
    {
      LOG = Log.Get(this);
    }

    public InputDisableHandle DisableInput()
    {
      _inputDisableCounter++;
      return new InputDisableHandle(this);
    }

    public sealed class InputDisableHandle
    {
      private readonly CharacterInput _input;
      private bool _released;

      internal InputDisableHandle(CharacterInput input)
      {
        _input = input;
      }

      public void Release()
      {
        if (_released) return;
        _released = true;
        _input._inputDisableCounter--;
      }
    }

    public void SetInput(Vector2 value)
    {
      Input = value.normalized;
      InputDirection = InputDirectionExtensions.Parse(Input);
      if (InputDisabled) return;
      SetMovement(Input);
    }
    
    public void SetMovement(Vector2 value)
    {
      Movement = value.normalized;
      MovementDirection = InputDirectionExtensions.Parse(Input);

      if (!facingLocked && Movement.x != 0)
      {
        var facing = Facing;
        facing.x = Movement.x > 0 ? 1 : -1;
        Facing = facing;
      }
      onInputMovement.Invoke(Movement);
    }
    
    public void OnAction(InputActionData actionData)
    {
      var action = actionData.action;
      var phase = actionData.phase;

      _pressingDict[action] = actionData.IsPressing(IsPressing(action));
      
      onInputActionAnyway.Invoke(actionData);

      if (debug)
      {
        LOG.D($"▶ {action} {phase} {actionData.timestamp}");
      }
      
      if (InputDisabled) return;

      _inputHistory.AddFirst(actionData);
      if (_inputHistory.Count >= 10)
      {
        _inputHistory.RemoveLast();
      }
      
      onInputAction.Invoke(actionData);
    }

    public bool IsPressing(string action)
    {
      return _pressingDict.ContainsKey(action) && _pressingDict[action];
    }
    
    public void ConsumeInputAction(string action, EInputPhase phase)
    {
      var node = _inputHistory.First;
      while (node != null) {
        var nextNode = node.Next;
        var value = node.Value;
        if (value.action == action && value.phase == phase)
        {
          node.Value.consumed = true;
          _inputHistory.Remove(node);
        }
        node = nextNode;
      }
    }

    public bool IsCoyoteTime(string action, EInputPhase phase, float delta, float time)
    {
      return GetCoyoteTimeAction(action, phase, delta, time) != null;
    }

    public InputActionData GetCoyoteTimeAction(string action, EInputPhase phase, float delta, float time)
    {
      var inputActionData = _inputHistory.FirstOrDefault(item => item.action == action && item.phase == phase);
      if (inputActionData == null)
      {
        return null;
      }
      return time - inputActionData.timestamp <= delta ? inputActionData : null;
    }
  }
}