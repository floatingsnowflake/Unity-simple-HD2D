using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamMingo.Ase.Editor.Settings;
using TeamMingo.ImagePacker.Editor;
using MonoGame.Aseprite.ContentPipeline;
using MonoGame.Aseprite.ContentPipeline.Models;
using Unity.Collections;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace TeamMingo.Ase.Editor.Processors
{
  public class AnimationImportProcessor : AseImporter.GenericProcessor<AnimationImportSettings>, IDisposable
  {
    protected override void Process(AseImporter importer, AssetImportContext ctx, AnimationImportSettings settings, AsepriteDocument document)
    {
    }

    protected override void Postprocess(AseImporter importer, AssetImportContext ctx, AnimationImportSettings[] settingsArray,
      AsepriteDocument document)
    {
      // index => frame
      var frameToIndexDict = new Dictionary<int, int>();
      var colorNativeArray = new List<NativeArray<Color32>>();
      var packedNativeColors = default(NativeArray<Color32>);
      
      try
      {
        foreach (var tagData in document.Tags)
        {
          var settings = settingsArray.FirstOrDefault(_ => _.tag == tagData.Name);
          if (settings != null)
          {
            for (var i = tagData.From; i <= tagData.To; i++)
            {
              if (frameToIndexDict.ContainsValue(i)) continue;
              frameToIndexDict[i] = colorNativeArray.Count;
              
              var frame = document.Frames[i];
              var pixels = AseUtils.FlattenFrame(document, frame);
              var colors = pixels.ToColor32Array();
              var nativeColors = new NativeArray<Color32>(colors, Allocator.Temp);
              colorNativeArray.Add(nativeColors);
            }
          }
        }

        int width, height;
        RectInt[] spriteRects;
        Vector2Int[] uvTransform;

        ImagePackUtils.Pack(
          colorNativeArray.ToArray(),
          document.Header.Width, document.Header.Height,
          1,
          tight: false,
          out packedNativeColors,
          out width, out height,
          out spriteRects, out uvTransform);

        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        var png = ImageConversion.EncodeNativeArrayToPNG(
          packedNativeColors,
          GraphicsFormat.R8G8B8A8_SRGB,
          (uint) width,
          (uint) height);
        if (!texture.LoadImage(png.ToArray()))
        {
          Debug.LogWarning("Texture loading not successful");
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.alphaIsTransparency = true;
        texture.name = $"{Path.GetFileNameWithoutExtension(ctx.assetPath)}_animations_texture";
        ctx.AddObjectToAsset(texture.name, texture);

        var frameIdx = 0;
        var spriteDict = new Dictionary<int, Sprite>();
        
        foreach (var tagData in document.Tags)
        {
          var sprites = new List<Sprite>();
          var frameDurations = new List<float>();
          var settings = settingsArray.FirstOrDefault(_ => _.tag == tagData.Name);
          if (settings != null)
          {
            for (var i = tagData.From; i <= tagData.To; i++)
            {
              frameDurations.Add(document.Frames[i].Duration / 1000f);
              if (spriteDict.ContainsKey(i))
              {
                sprites.Add(spriteDict[i]);
              }
              else
              {
                var spriteName = $"{tagData.Name}_{i - tagData.From}";
                var spriteRectInt = spriteRects[frameToIndexDict[i]];
                var spriteRect = new Rect(spriteRectInt.x, spriteRectInt.y, spriteRectInt.width, spriteRectInt.height);
                var sprite = Sprite.Create(texture, spriteRect, settings.pivot, settings.pixelsPerUnit);
                sprite.name = spriteName;
                ctx.AddObjectToAsset(sprite.name, sprite);
                sprites.Add(sprite);
                frameIdx++;
              }
            }
            
            if (sprites.Count > 0)
            {
              var clip = GenerateAnimationClip(tagData.Name, settings.loop, sprites.ToArray(), frameDurations.ToArray(), "");
              ctx.AddObjectToAsset(clip.name, clip);
            }
          }
        }
      }
      finally
      {
        packedNativeColors.Dispose();
        foreach (var nativeArray in colorNativeArray)
        {
          nativeArray.Dispose();
        }
      }
    }

    private AnimationClip GenerateAnimationClip(
      string animationName,
      bool isLoop,
      IReadOnlyList<Sprite> sprites,
      IReadOnlyList<float> frameDurations,
      string objectPath)
    {
      var clip = new AnimationClip();
      var curveBinding = new EditorCurveBinding
      {
        path = objectPath,
        type = typeof(SpriteRenderer),
        propertyName = "m_Sprite"
      };

      var keyFrames = new List<ObjectReferenceKeyframe>();

      float time = 0;

      for (var i = 0; i < sprites.Count; i++)
      {
        keyFrames.Add(new ObjectReferenceKeyframe
        {
          time = time,
          value = sprites[i]
        });

        time += frameDurations[i];
      }

      keyFrames.Add(new ObjectReferenceKeyframe
      {
        time = time - (1f / clip.frameRate),
        value = sprites[sprites.Count - 1]
      });


      clip.name = animationName;

      if (isLoop)
      {
        var set = AnimationUtility.GetAnimationClipSettings(clip);
        set.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, set);
      }

      AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames.ToArray());
      
      return clip;
    }
  }
}