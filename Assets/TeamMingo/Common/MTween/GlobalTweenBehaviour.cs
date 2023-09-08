using System;
using System.Collections.Generic;
using System.Linq;
using TeamMingo.Common.Runtime;
using TeamMingo.MTween;
using UnityEngine;

namespace TeamMingo.Common.MTween
{
  public class GlobalTweenBehaviour : PersistentSingleton<GlobalTweenBehaviour>
  {
    private readonly List<TweenInstance> _updateTweenList = new List<TweenInstance>();
    private readonly List<TweenInstance> _fixedUpdateTweenList = new List<TweenInstance>();

    private void Update()
    {
      var anyCompleted = false;
      foreach (var tweenInstance in _updateTweenList)
      {
        tweenInstance.Update(Time.deltaTime);
        if (tweenInstance.completed)
        {
          anyCompleted = true;
        }
      }

      if (anyCompleted)
      {
        foreach (var tweenInstance in _updateTweenList.Where(_ => _.completed).ToArray())
        {
          RemoveTween(tweenInstance);
        }
      }
    }

    private void FixedUpdate()
    {
      var anyCompleted = false;
      foreach (var tweenInstance in _fixedUpdateTweenList)
      {
        tweenInstance.Update(Time.fixedDeltaTime);
        if (tweenInstance.completed)
        {
          anyCompleted = true;
        }
      }

      if (anyCompleted)
      {
        foreach (var tweenInstance in _fixedUpdateTweenList.Where(_ => _.completed).ToArray())
        {
          RemoveTween(tweenInstance);
        }
      }
    }

    public void AddTween(TweenInstance tween)
    {
      if (tween.updateMode == ETweenUpdateMode.Update)
      {
        _updateTweenList.Add(tween);
      }
      else
      {
        _fixedUpdateTweenList.Add(tween);
      }
    }

    public void RemoveTween(TweenInstance tween)
    {
      if (tween.updateMode == ETweenUpdateMode.Update)
      {
        _updateTweenList.Remove(tween);
      }
      else
      {
        _fixedUpdateTweenList.Remove(tween);
      }
    }

    public void ClearAllTween(object target)
    {
      foreach (var tweenInstance in _updateTweenList.Where(_ => _.target == target).ToArray())
      {
        RemoveTween(tweenInstance);
      }
      foreach (var tweenInstance in _fixedUpdateTweenList.Where(_ => _.target == target).ToArray())
      {
        RemoveTween(tweenInstance);
      }
    }
    
  }
}