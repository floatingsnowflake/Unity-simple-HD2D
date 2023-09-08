using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TeamMingo.Ase.Editor.Settings;
using MonoGame.Aseprite.ContentPipeline.Models;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TeamMingo.Ase.Editor
{
  [ScriptedImporter(1, new []{ "ase", "aseprite"}, new string[0], 0)]
  public class AseImporter : ScriptedImporter
  {
    public interface IProcessor : IDisposable
    {
      void Preprocess(AseImporter importer, AssetImportContext ctx, AseImportSettings[] settings,
        AsepriteDocument document);
      void Process(AseImporter importer, AssetImportContext ctx, AseImportSettings settings, AsepriteDocument document);

      void Postprocess(AseImporter importer, AssetImportContext ctx, AseImportSettings[] settings,
        AsepriteDocument document);
    }

    public abstract class GenericProcessor<T> : IProcessor where T : AseImportSettings
    {
      public virtual void Dispose()
      {
      }

      public void Process(AseImporter importer, AssetImportContext ctx, AseImportSettings settings,
        AsepriteDocument document)
      {
        Process(importer, ctx, (T) settings, document);
      }

      public void Preprocess(AseImporter importer, AssetImportContext ctx, AseImportSettings[] settingsArray, AsepriteDocument document)
      {
        Preprocess(importer, ctx, settingsArray.Select(_ => (T) _).ToArray(), document);
      }

      public void Postprocess(AseImporter importer, AssetImportContext ctx, AseImportSettings[] settingsArray, AsepriteDocument document)
      {
        Postprocess(importer, ctx, settingsArray.Select(_ => (T) _).ToArray(), document);
      }

      protected virtual void Process(AseImporter importer, AssetImportContext ctx, T settings, AsepriteDocument document) {}
      protected virtual void Preprocess(AseImporter importer, AssetImportContext ctx, T[] settings, AsepriteDocument document) {}
      protected virtual void Postprocess(AseImporter importer, AssetImportContext ctx, T[] settings, AsepriteDocument document) {}
    }
    
    public static string GetAbsoluteAssetPath(AssetImportContext ctx)
    {
      return $"{Application.dataPath}{ctx.assetPath.Substring("Assets".Length)}";
    }

    [Flags]
    public enum EImportFlags
    {
      None = 0,
      LayerToSprite = 1,
      TagToAnimation = 2,
      Tileset = 4,
    }

    public EImportFlags importFlags = EImportFlags.None;
    public int pixelsPerUnit = 8;
    public Vector2 pivot = new Vector2(0.5f, 0.5f);

    [Space]
    public LayerImportSettings[] layersImporting;

    [Space] 
    public AnimationImportSettings[] animationsImporting;
    
    [Space] 
    public TilesetImportSettings[] tilesetImporting;

    public void ImportAsset(AssetImportContext ctx, Object asset)
    {
      ctx.AddObjectToAsset(asset.name, asset);
    }
    
    public override void OnImportAsset(AssetImportContext ctx)
    {
      var assetFullPath = GetAbsoluteAssetPath(ctx);
      var doc = AsepriteDocument.FromFile(assetFullPath);

      if (layersImporting != null && importFlags.HasFlag(EImportFlags.LayerToSprite))
      {
        RunProcessor(ctx, layersImporting, doc);
      }

      if (animationsImporting != null && importFlags.HasFlag(EImportFlags.TagToAnimation))
      {
        RunProcessor(ctx, animationsImporting, doc);
      }
      
      if (tilesetImporting != null && importFlags.HasFlag(EImportFlags.Tileset))
      {
        RunProcessor(ctx, tilesetImporting, doc);
      }
      
      var gameObject = new GameObject(Path.GetFileNameWithoutExtension(assetPath));
      gameObject.transform.parent = null;
      ctx.AddObjectToAsset(gameObject.name, gameObject);
      ctx.SetMainObject(gameObject);
    }

    private void RunProcessor<T>(AssetImportContext ctx, T[] settingsArray,
      AsepriteDocument doc) where T : AseImportSettings
    {
      var attribute = typeof(T).GetCustomAttribute<AseProcessorAttribute>();
      if (attribute == null)
      {
        return;
      }
      if (Activator.CreateInstance(attribute.type) is IProcessor processor)
      {
        try
        {
          RunProcessor(ctx, settingsArray, doc, processor);
        }
        finally
        {
          processor.Dispose();
        }
      }
    }

    private void RunProcessor<T>(AssetImportContext ctx, T[] settingsArray, AsepriteDocument doc, IProcessor processor) where T : AseImportSettings
    {
      foreach (var aseImportSettings in settingsArray)
      {
        if (!aseImportSettings.overrides.HasFlag(EAseImportOverrides.PixelsPerUnit))
        {
          aseImportSettings.pixelsPerUnit = pixelsPerUnit;
        }
        if (!aseImportSettings.overrides.HasFlag(EAseImportOverrides.Pivot))
        {
          aseImportSettings.pivot = pivot;
        }
      }
      processor.Preprocess(this, ctx, settingsArray.Select(_ => (AseImportSettings) _).ToArray(), doc);
      foreach (var aseImportSettings in settingsArray)
      {
        processor.Process(this, ctx, aseImportSettings, doc);
      }
      processor.Postprocess(this, ctx, settingsArray.Select(_ => (AseImportSettings) _).ToArray(), doc);
    }
  }
}