using System;
using DG.Tweening;
using TeamMingo.Characters.Runtime;
using UnityEngine;

namespace HD2DProject2.Scripts
{
  public class SwitchPerspective : MonoBehaviour
  {
    public float duration;
    public float step;
    public Ease ease;

    public float CurrentAngle => _angle;
    
    private float _angle;
    private Tweener _tweener;

    private void Awake()
    {
      var input = GetComponent<CharacterInput>();
      input.onInputAction.AddListener((actionData) =>
      {
        if (actionData.IsActionDown("SwitchPerspective"))
        {
          Switch();
        }
      });
    }

    public void Switch()
    {
      _angle += step;

      if (_tweener != null)
      {
        _tweener.Kill();
      }
      _tweener = transform.DOLocalRotate(new Vector3(0, _angle, 0), duration)
        .SetEase(ease)
        .OnComplete(() => {
        _tweener = null;
      });
    }
  }
}