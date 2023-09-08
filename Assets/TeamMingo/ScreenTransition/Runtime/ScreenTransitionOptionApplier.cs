using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  public abstract class ScreenTransitionOptionApplier : MonoBehaviour
  {
    
    public Transform target;
    
    protected virtual void Awake()
    {
      if (!target) target = transform;
      var transition = GetComponentInParent<ScreenTransitionBase>();
      transition.beforeEnter.AddListener(OnBeforeEnter);
      transition.beforeExit.AddListener(OnBeforeExit);
      transition.prepareEnter.AddListener(OnPrepareEnter);
      transition.prepareExit.AddListener(OnPrepareExit);
    }

    private void OnBeforeExit(IScreenTransitionOptions options)
    {
      ApplyOptions(EScreenTransitionAction.Enter, options);
    }

    private void OnBeforeEnter(IScreenTransitionOptions options)
    {
      ApplyOptions(EScreenTransitionAction.Exit, options);
    }
    
    private void OnPrepareEnter(IScreenTransitionOptions options)
    {
      ApplyOptions(EScreenTransitionAction.PrepareEnter, options);
    }

    private void OnPrepareExit(IScreenTransitionOptions options)
    {
      ApplyOptions(EScreenTransitionAction.PrepareExit, options);
    }

    public abstract void ApplyOptions(EScreenTransitionAction action, IScreenTransitionOptions options);
  }
}