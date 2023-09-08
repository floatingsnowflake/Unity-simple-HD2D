using System;
using System.Linq;
using TeamMingo.Ase.Editor.Settings;
using MonoGame.Aseprite.ContentPipeline;
using MonoGame.Aseprite.ContentPipeline.Models;
using MonoGame.Aseprite.ContentPipeline.ThirdParty.Pixman;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace TeamMingo.Ase.Editor.Processors
{
  public class LayerImportProcessor : AseImporter.GenericProcessor<LayerImportSettings>
  {
    protected override void Process(AseImporter importer, AssetImportContext ctx, LayerImportSettings settings, AsepriteDocument document)
    {
      var layerIndex = document.Layers.FindIndex(_ => _.Name == settings.layer);
      if (layerIndex == -1) return;

      var layer = document.Layers[layerIndex];
      var cel = document.Frames.First().Cels.First(_ => _.LayerIndex == layerIndex);
      var texture = new Texture2D(document.Header.Width, document.Header.Height, TextureFormat.ARGB32, mipChain: false);
      texture.name = $"{settings.layer}_texture";
      texture.wrapMode = TextureWrapMode.Clamp;
      texture.filterMode = FilterMode.Point;
      texture.alphaIsTransparency = true;
      
      uint[] framePixels = new uint[document.Header.Width * document.Header.Height];
      byte opacity = Combine32.MUL_UN8(cel.Opacity, layer.Opacity);
      for (int p = 0; p < cel.Pixels.Length; p++)
      {
        int x = (p % cel.Width) + cel.X;
        int y = (p / cel.Width) + cel.Y;
        int index = (document.Header.Height - y - 1) * document.Header.Width + x;
        if (index < 0 || index >= framePixels.Length)
        {
          continue;
        }
        uint backdrop = framePixels[index];
        uint src = cel.Pixels[p];

        Func<uint, uint, int, uint> blender = Utils.GetBlendFunction(layer.BlendMode);
        uint blendedColor = blender.Invoke(backdrop, src, opacity);

        framePixels[index] = blendedColor;
      }
      var colors = framePixels.ToColor32Array();
      texture.SetPixels32(colors);
      texture.Apply();
      
      importer.ImportAsset(ctx, texture);

      var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), settings.pivot,
        settings.pixelsPerUnit);
      sprite.name = $"{settings.layer}_sprite";
      importer.ImportAsset(ctx, sprite);
    }
  }
}