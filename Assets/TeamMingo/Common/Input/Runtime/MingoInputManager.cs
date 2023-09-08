using System.Collections;
using TeamMingo.Common.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace TeamMingo.Input.Runtime
{
  public class MingoInputManager : PersistentSingleton<MingoInputManager>
  {
    public UnityEvent<EInputActionMap> onInputActionMapSwitch;
    public UnityEvent<InputActionData> onInputAction;

    protected override void Setup()
    {
      base.Setup();
      if (onInputAction == null)
      {
        onInputAction = new UnityEvent<InputActionData>();
      }

      if (onInputActionMapSwitch == null)
      {
        onInputActionMapSwitch = new UnityEvent<EInputActionMap>();
      }
    }

    public void SwitchInputActionMap(EInputActionMap inputActionMap)
    {
      onInputActionMapSwitch.Invoke(inputActionMap);
    }
    
    public IEnumerator WaitInputAction(string action, EInputPhase phase)
    {
      var completed = false;
      void OnPlayerInputAction (InputActionData actionData)
      {
        if (actionData.action == action && actionData.phase == phase)
        {
          completed = true;
        }
      }
      onInputAction.AddListener(OnPlayerInputAction);
      yield return new WaitUntil(() => completed);
      onInputAction.RemoveListener(OnPlayerInputAction);
    }
  }
}