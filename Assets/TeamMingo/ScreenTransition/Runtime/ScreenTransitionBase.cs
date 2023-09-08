using System;
using System.Collections;
using TeamMingo.Common.Runtime.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace TeamMingo.ScreenTransition.Runtime
{
  public abstract class ScreenTransitionBase : MonoBehaviour
  {
    
    public UnityEvent<IScreenTransitionOptions> prepareEnter;
    public UnityEvent<IScreenTransitionOptions> prepareExit;
    public UnityEvent<IScreenTransitionOptions> beforeEnter;
    public UnityEvent<IScreenTransitionOptions> afterEnter;
    public UnityEvent<IScreenTransitionOptions> beforeExit;
    public UnityEvent<IScreenTransitionOptions> afterExit;
    
    public IEnumerator OnEnter(IScreenTransitionOptions options)
    {
      beforeEnter.Invoke(options);
      yield return ProcessEnter(options);
      afterEnter.Invoke(options);
    }

    internal void PrepareEnterInternal(IScreenTransitionOptions options)
    {
      prepareEnter.Invoke(options);
      PrepareEnter(options);
    }

    protected abstract void PrepareEnter(IScreenTransitionOptions options);
    
    protected abstract IEnumerator ProcessEnter(IScreenTransitionOptions options);

    public IEnumerator OnExit(IScreenTransitionOptions options)
    {
      beforeExit.Invoke(options);
      yield return ProcessExit(options);
      afterExit.Invoke(options);
    }

    internal void PrepareExitInternal(IScreenTransitionOptions options)
    {
      prepareExit.Invoke(options);
      PrepareExit(options);
    }
    
    protected abstract void PrepareExit(IScreenTransitionOptions options);
    protected abstract IEnumerator ProcessExit(IScreenTransitionOptions options);

    protected void Track(Transform trans, string trackTag)
    {
      if (trackTag.IsNullOrWhitespace()) return;
      var obj = GameObject.FindWithTag(trackTag);
      if (!obj) return;
      var trackTrans = obj.transform;
      if (trans is RectTransform rectTrans)
      {
        var canvas = trans.GetComponentInParent<Canvas>();
        var canvasRect = canvas.GetComponent<RectTransform>();
        var cam = canvas.worldCamera;
        Vector2 viewportPos = cam.WorldToViewportPoint(trackTrans.position);
        var sizeDelta = canvasRect.sizeDelta;
        var screenPos = new Vector2(
          ((viewportPos.x*sizeDelta.x)-(sizeDelta.x*0.5f)),
          ((viewportPos.y*sizeDelta.y)-(sizeDelta.y*0.5f)));
    
        rectTrans.anchoredPosition = screenPos;
      }
      else
      {
        trans.position = trackTrans.position;
      }
    }
  }
}