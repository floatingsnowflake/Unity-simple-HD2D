using System;
using UnityEngine;

namespace TeamMingo.Ase.Editor
{
  [Serializable]
  public abstract class AseImportSettings
  {
    public EAseImportOverrides overrides = EAseImportOverrides.None;
    public int pixelsPerUnit = 8;
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
  }

  [Flags]
  public enum EAseImportOverrides
  {
    None = 0,
    PixelsPerUnit = 1,
    Pivot = 2
  }
}