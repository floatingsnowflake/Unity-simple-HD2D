using MonoGame.Aseprite.ContentPipeline.Serialization;

namespace MonoGame.Aseprite.ContentPipeline.Models
{
  /// <summary>
  ///   A list of external files linked with this file. It might be used to reference external palettes or tilesets.
  /// </summary>
  /// <code>
  ///   DWORD       Number of entries
  ///   BYTE[8]     Reserved (set to zero)
  ///   + For each entry
  ///   DWORD     Entry ID (this ID is referenced by tilesets or palettes)
  ///   BYTE[8]   Reserved (set to zero)
  ///   STRING    External file name
  /// </code>
  public class AsepriteExternalFileChunk : AsepriteChunk
  {
    public class Entry
    {
      public readonly uint ID; 
      public readonly string FileName;

      public Entry(uint id, string fileName)
      {
        ID = id;
        FileName = fileName;
      }
    }

    public Entry[] Entries { get; private set; }
    
    internal AsepriteExternalFileChunk(AsepriteReader reader)
    {
      var entries = reader.ReadDWORD();
      Entries = new Entry[entries];
      for (var i = 0; i < entries; i++)
      {
        var id = reader.ReadDWORD();
        reader.Ignore(8);
        var fileName = reader.ReadString();
        Entries[i] = new Entry(id, fileName);
      }
    }
  }
}