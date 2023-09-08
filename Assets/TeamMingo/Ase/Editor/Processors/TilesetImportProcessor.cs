using System;
using System.Linq;
using MonoGame.Aseprite.ContentPipeline;
using MonoGame.Aseprite.ContentPipeline.Models;
using MonoGame.Aseprite.ContentPipeline.ThirdParty.Aseprite;
using MonoGame.Aseprite.ContentPipeline.ThirdParty.Pixman;
using TeamMingo.Ase.Editor.Settings;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamMingo.Ase.Editor.Processors
{
  public class TilesetImportProcessor : AseImporter.GenericProcessor<TilesetImportSettings>
  {
    protected override void Process(AseImporter importer, AssetImportContext ctx, TilesetImportSettings settings, AsepriteDocument document)
    {
      var tilesetChunk = document.Tilesets.FirstOrDefault(_ => _.Name == settings.tileset);
      if (tilesetChunk == null) return;

      var texture = new Texture2D(tilesetChunk.TileWidth, (int) (tilesetChunk.TileHeight * tilesetChunk.NumberOfTiles), TextureFormat.ARGB32, mipChain: false);
      texture.name = $"{settings.tileset}_texture";
      texture.alphaIsTransparency = true;
      texture.wrapMode = TextureWrapMode.Clamp;
      texture.filterMode = FilterMode.Point;
      
      uint[] framePixels = new uint[tilesetChunk.Pixels.Length];
      byte opacity = 255;
      for (int p = 0; p < tilesetChunk.Pixels.Length; p++)
      {
        var tileW = tilesetChunk.TileWidth;
        int x = (p % tileW) + 0;
        int y = (p / tileW) + 0;
        int index = (texture.height - y - 1) * texture.width + x;
        if (index < 0 || index >= framePixels.Length)
        {
          continue;
        }
        uint backdrop = framePixels[index];
        uint src = tilesetChunk.Pixels[p];

        Func<uint, uint, int, uint> blender = BlendFuncs.rgba_blender_normal;
        uint blendedColor = blender.Invoke(backdrop, src, opacity);

        framePixels[index] = blendedColor;
      }
      var colors = framePixels.ToColor32Array();
      texture.SetPixels32(colors);
      texture.Apply();
      
      ctx.AddObjectToAsset(texture.name, texture);

      for (var i = 0; i < tilesetChunk.NumberOfTiles; i++)
      {
        var rect = new Rect(
          0, (tilesetChunk.NumberOfTiles - i - 1) * tilesetChunk.TileHeight, 
          tilesetChunk.TileWidth,
          tilesetChunk.TileHeight);
        var sprite = Sprite.Create(texture,
          rect, settings.pivot,
          settings.pixelsPerUnit, extrude: 0, SpriteMeshType.FullRect);
        sprite.name = $"{settings.tileset}_sprite{i}";
        ctx.AddObjectToAsset(sprite.name, sprite);

        if (settings.generateTiles)
        {
          var tile = ScriptableObject.CreateInstance<Tile>();
          tile.name = $"{settings.tileset}_tile{i}";
          tile.sprite = sprite;
          tile.color = settings.tileColor;
          tile.colliderType = settings.tileColliderType;
          ctx.AddObjectToAsset(tile.name, tile);
        }
      }
    }
  }
}