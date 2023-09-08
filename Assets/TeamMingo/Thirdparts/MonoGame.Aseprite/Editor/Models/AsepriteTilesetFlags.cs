namespace MonoGame.Aseprite.ContentPipeline.Models
{
  [System.Flags]
  public enum AsepriteTilesetFlags
  {
    None = 0,
    IncludeExternalFile = 1,
    IncludeTilesInThisFIle = 2,
    UsingZeroAsEmptyTile = 4,
  }
}