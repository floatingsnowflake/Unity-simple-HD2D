using System;
using MonoGame.Aseprite.ContentPipeline;
using MonoGame.Aseprite.ContentPipeline.Models;
using MonoGame.Aseprite.ContentPipeline.ThirdParty.Pixman;

namespace TeamMingo.Ase.Editor
{
  public static class AseUtils
  {
    /// <summary>
    ///     Given an <see cref="AsepriteFrame"/> instance, combines all Cels
    ///     of the frame into a single array of packed color values representing
    ///     all pixels within the frame.
    /// </summary>
    /// <param name="frame">
    ///     The <see cref="AsepriteFrame"/> instance to flatten.
    /// </param>
    /// <param name="onlyVisibleLayers">
    ///     A value indicating if only visible layers should be processed.
    /// </param>
    /// <returns></returns>
    public static uint[] FlattenFrame(AsepriteDocument document, AsepriteFrame frame, bool onlyVisibleLayers = true)
    {
      uint[] framePixels = new uint[document.Header.Width * document.Header.Height];

      for (int c = 0; c < frame.Cels.Count; c++)
      {
        AsepriteCelChunk cel = frame.Cels[c];

        if (cel.LinkedCel != null)
        {
          cel = cel.LinkedCel;
        }

        AsepriteLayerChunk layer = document.Layers[cel.LayerIndex];

        if ((layer.Flags & AsepriteLayerFlags.Visible) != 0 || !onlyVisibleLayers)
        {
          byte opacity = Combine32.MUL_UN8(cel.Opacity, layer.Opacity);

          for (int p = 0; p < cel.Pixels.Length; p++)
          {
            int x = (p % cel.Width) + cel.X;
            int y = (p / cel.Width) + cel.Y;
            int index = (document.Header.Height - y - 1) * document.Header.Width + x;

            //  Sometimes a cell can have a negative x and/or y value. This is caused
            //  by selecting an area within aseprite and then moving a portion of the
            //  selected pixels outside the canvas.  We don't care about these pixels
            //  so if the index is outside the range of the array to store them in
            //  then we'll just ignore them.
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
        }
      }

      return framePixels;
    }
  }
}