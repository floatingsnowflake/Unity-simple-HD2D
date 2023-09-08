using System;
using UnityEngine;

namespace HD2DProject.Scripts.Sprites
{
  [ExecuteAlways]
  public class SpriteSorting : MonoBehaviour
  {
    
    private void LateUpdate()
    {
      if (TryGetComponent(out SpriteRenderer spriteRenderer))
      {
        spriteRenderer.sortingOrder = Mathf.CeilToInt(transform.position.z * 16);
      }
    }
  }
}