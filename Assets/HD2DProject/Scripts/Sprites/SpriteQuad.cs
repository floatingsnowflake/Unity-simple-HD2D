using System;
using UnityEngine;

namespace HD2DProject.Scripts.Sprites
{
  public class SpriteQuad : MonoBehaviour
  {
    public Sprite sprite;
    public MeshRenderer meshRenderer;
    public MeshFilter filter;

    private Sprite _previousSprite;

    private void Awake()
    {
      SetSprite(sprite);
    }

    public void SetSprite(Sprite newSprite)
    {
      if (!meshRenderer)
      {
        TryGetComponent(out meshRenderer);
      }

      if (!filter)
      {
        TryGetComponent(out filter);
      }
      var material = meshRenderer.material;
      if (sprite)
      {
        material.mainTexture = newSprite.texture;
        filter.mesh.uv = newSprite.uv;
      }
      else
      {
        material.mainTexture = null;
      }
    }
    
    private void Update()
    {
      if (SpriteRecentlyChanged())
      {
        SetSprite(sprite);
      }
    }
 
    private bool SpriteRecentlyChanged()
    {
      if (sprite != _previousSprite)
      {
        _previousSprite = sprite;
        return true;
      } else
      {
        return false;
      }
    }
  }
}