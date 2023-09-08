using System;
using System.IO;
using System.IO.Compression;
using MonoGame.Aseprite.ContentPipeline.Serialization;
using UnityEngine;

namespace MonoGame.Aseprite.ContentPipeline.Models
{
  
  /// <summary>
  /// </summary>
  /// <code>
  /// DWORD       Tileset ID
  /// DWORD       Tileset flags
  /// 1 - Include link to external file
  /// 2 - Include tiles inside this file
  /// 4 - Tilemaps using this tileset use tile ID=0 as empty tile
  /// (this is the new format). In rare cases this bit is off,
  /// and the empty tile will be equal to 0xffffffff (used in
  /// internal versions of Aseprite)
  /// DWORD       Number of tiles
  /// WORD        Tile Width
  /// WORD        Tile Height
  /// SHORT       Base Index: Number to show in the screen from the tile with
  /// index 1 and so on (by default this is field is 1, so the data
  /// that is displayed is equivalent to the data in memory). But it
  /// can be 0 to display zero-based indexing (this field isn't used
  /// for the representation of the data in the file, it's just for
  /// UI purposes).
  /// BYTE[14]    Reserved
  /// STRING      Name of the tileset
  /// + If flag 1 is set
  /// DWORD     ID of the external file. This ID is one entry
  /// of the the External Files Chunk.
  /// DWORD     Tileset ID in the external file
  /// + If flag 2 is set
  /// DWORD     Compressed data length
  /// PIXEL[]   Compressed Tileset image (see NOTE.3):
  /// (Tile Width) x (Tile Height x Number of Tiles)
  /// </code>
  public class AsepriteTilesetChunk : AsepriteChunk
  {
    public uint ID { get; private set; }
    public AsepriteTilesetFlags Flags { get; private set; }
    public uint NumberOfTiles { get; private set; }
    public ushort TileWidth { get; private set; }
    public ushort TileHeight { get; private set; }
    public short BaseIndex { get; private set; }
    public string Name { get; private set; }
    public uint ExternalFileID { get; private set; }
    public uint ExternalTilesetID { get; private set; }
    
    /// <summary>
    ///     Gets the raw pixel data contained in this cel.
    /// </summary>
    public byte[] PixelData { get; private set; }
    
    /// <summary>
    ///     Gets an array of packed uint values for this tileset that
    ///     represents the pixels.
    /// </summary>
    public uint[] Pixels { get; private set; }
    
    
    internal AsepriteTilesetChunk(AsepriteReader reader, AsepriteDocument document, uint dataSize)
    {
      ID = reader.ReadDWORD();
      Flags = (AsepriteTilesetFlags) reader.ReadDWORD();
      NumberOfTiles = reader.ReadDWORD();
      TileWidth = reader.ReadWORD();
      TileHeight = reader.ReadWORD();
      BaseIndex = reader.ReadSHORT();
      reader.Ignore(14);
      Name = reader.ReadString();

      if (string.IsNullOrWhiteSpace(Name))
      {
        Name = $"Tileset_{ID}";
      }

      if (Flags.HasFlag(AsepriteTilesetFlags.IncludeExternalFile))
      {
        ExternalFileID = reader.ReadDWORD();
        ExternalTilesetID = reader.ReadDWORD();
      }

      if (Flags.HasFlag(AsepriteTilesetFlags.IncludeTilesInThisFIle))
      {
        uint dataLength = reader.ReadDWORD();
        
        //  Calculate the remaning data to read in the cel chunk
        long bytesToRead = dataLength;

        //  Read the remaning bytes into a buffer
        byte[] buffer = reader.ReadBytes((int) bytesToRead);

        //  For compressed, we need to deflate the buffer. First, we'll put it in a
        //  memory stream to work with
        MemoryStream compressedStream = new MemoryStream(buffer);

        //  The first 2 bytes of the compressed stream are the zlib header informaiton,
        //  and we need to ignore them before we attempt to deflate
        _ = compressedStream.ReadByte();
        _ = compressedStream.ReadByte();

        //  Now we can deflate the compressed stream
        using (MemoryStream decompressedStream = new MemoryStream())
        {
          using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
          {
            deflateStream.CopyTo(decompressedStream);
            PixelData = decompressedStream.ToArray();
          }
        }

        Pixels = new uint[TileWidth * TileHeight * NumberOfTiles];
        if (document.Header.ColorDepth == AsepriteColorDepth.RGBA)
        {
          for (int i = 0, b = 0; i < Pixels.Length; i++, b += 4)
          {
            Pixels[i] = Utils.BytesToPacked(PixelData[b], PixelData[b + 1], PixelData[b + 2], PixelData[b + 3]);
          }
        }
        else if (document.Header.ColorDepth == AsepriteColorDepth.Grayscale)
        {
          for (int i = 0, b = 0; i < Pixels.Length; i++, b += 2)
          {
            Pixels[i] = Utils.BytesToPacked(PixelData[b], PixelData[b], PixelData[b], PixelData[b + 1]);
          }
        }
        else if (document.Header.ColorDepth == AsepriteColorDepth.Indexed)
        {
          for (int i = 0; i < Pixels.Length; i++)
          {
            int paletteIndex = PixelData[i];
            if (paletteIndex == document.Header.TransparentIndex)
            {
              Pixels[i] = Utils.BytesToPacked(0, 0, 0, 0);
            }
            else
            {
              AsepritePaletteColor paletteColor = document.Palette.Colors[paletteIndex];
              Pixels[i] = Utils.BytesToPacked(paletteColor.Red, paletteColor.Green, paletteColor.Blue,
                paletteColor.Alpha);
            }
          }
        }
        else
        {
          throw new Exception($"Unrecognized color depth mode. {document.Header.ColorDepth}");
        }
      }
    }
  }
}