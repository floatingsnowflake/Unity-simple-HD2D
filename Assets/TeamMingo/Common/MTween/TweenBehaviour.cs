using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TeamMingo.MTween;

namespace TeamMingo.Tween {

  public class TweenBehaviour : MonoBehaviour {

    private TweenInstance mInstance;
    
    public TweenInstance SetTween(TweenInstance tween)
    {
      mInstance = tween;
      return mInstance;
    }

    public TweenInstance CreateTween(TweenInstance.ITargetWrapper wrapper)
    {
      mInstance = new TweenInstance(wrapper);
      return mInstance;
    }

    private void Update()
    {
      if (mInstance.updateMode != ETweenUpdateMode.Update) return;
      var time = mInstance.updateTime == ETweenUpdateTime.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
      if (!mInstance.completed && mInstance.Update(time)) {
        Destroy(this);
      }
    }

    private void FixedUpdate() {
      if (mInstance.updateMode != ETweenUpdateMode.FixedUpdate) return;
      var time = mInstance.updateTime == ETweenUpdateTime.Normal ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime;
      if (!mInstance.completed && mInstance.Update(time)) {
        Destroy(this);
      }
    }

  }

}