using TeamMingo.Common.Runtime.Extensions;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  public class ScreenTransition : MonoBehaviour
  {
    [ScreenTransitionSelector]
    public string transition;

    public void Enter()
    {
      ScreenTransitions.Instance.EnterTransition(transition);
    }

    public void Exit()
    {
      
      ScreenTransitions.Instance.ExitTransition(transition);
    }
  }
}