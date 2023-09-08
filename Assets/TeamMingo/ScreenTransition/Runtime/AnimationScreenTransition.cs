using System;
using System.Collections;
using TeamMingo.Common.Runtime.Extensions;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  public class AnimationScreenTransition : ScreenTransitionGeneric<AnimationScreenTransition.Options>
  {
    [Serializable]
    [SerializeReferenceDropdownName("Animation")]
    public class Options : IScreenTransitionOptions
    {
      public float speed = 1;
    }
    
    private Animator _animator;
  
    private void Awake()
    {
      _animator = GetComponent<Animator>();
    }
  
    protected override void PrepareEnter(Options options)
    {
      _animator.Play($"Enter");
      _animator.speed = 0;
    }
  
    protected override void PrepareExit(Options options)
    {
      _animator.Play($"Exit");
      _animator.speed = 0;
    }
  
    protected override IEnumerator ProcessEnter(Options options)
    {
      _animator.speed = options.speed;
      _animator.Play($"Enter");
      yield return new WaitUntil(() => _animator.IsAnimationEnd("Enter"));
    }
  
    protected override IEnumerator ProcessExit(Options options)
    {
      _animator.speed = options.speed;
      _animator.Play($"Exit");
      yield return new WaitUntil(() => _animator.IsAnimationEnd("Exit"));
    }
  }
}