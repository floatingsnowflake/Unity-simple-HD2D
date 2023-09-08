using TeamMingo.Common.Runtime;
using TeamMingo.Common.Runtime.Extensions;
using TeamMingo.Input.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamMingo.Characters.Runtime
{
  public class PlayerInputConnect : MonoBehaviour
  {
    private Log LOG;
    public bool debug;
    public CharacterInput characterInput;
    public PlayerInput playerInput;

    private void Awake()
    {
      LOG = Log.Get(this);
      playerInput.currentActionMap.actionTriggered += OnAction;
    }

    public void SwitchInputActionMap(EInputActionMap actionMap)
    {
      playerInput.SwitchCurrentActionMap(actionMap.Name());
    }

    private void OnDestroy()
    {
      if (Application.isEditor) return;
      playerInput.currentActionMap.actionTriggered -= OnAction;
    }

    private void OnAction(InputAction.CallbackContext context)
    {
      if (context.action.name == "Move")
      {
        characterInput.SetInput(context.ReadValue<Vector2>());
        return;
      }
      
      if (debug)
      {
        LOG.D($"OnAction {context.action.name} {context.action.phase}");
      }
      
      var actionName = context.action.name;
      var action = actionName;
      var phase = EInputPhase.Down;
      if (context.action.phase == InputActionPhase.Started)
      {
        phase = EInputPhase.Down;
      } 
      else if (context.action.phase == InputActionPhase.Canceled)
      {
        phase = EInputPhase.Up;
      }
      else if (context.action.phase == InputActionPhase.Waiting)
      {
        phase = EInputPhase.Pressing;
      }
      else
      {
        return;
      }

      var actionData = new InputActionData()
      {
        action = action,
        phase = phase,
        timestamp = Time.fixedTime,
        input = characterInput.Input,
      };
      
      characterInput.OnAction(actionData);
      MingoInputManager.Instance.onInputAction.Invoke(actionData);
    }
  }
}