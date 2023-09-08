/* ------------------------------------------------------------------------------
    Copyright (c) 2022 Christopher Whitley

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
------------------------------------------------------------------------------ */

using System;
using MonoGame.Aseprite.ContentPipeline.Models;
using MonoGame.Aseprite.ContentPipeline.ThirdParty.Aseprite;
using UnityEngine;

namespace MonoGame.Aseprite.ContentPipeline
{
    /// <summary>
    ///     A static class with utility functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        ///     Converts the array of 32-bit unsigned integers into an
        ///     array of XNA Color instances.
        /// </summary>
        /// <param name="packedValues">
        ///     An array of 32-bit unsigned integers, where each index contains
        ///     the packed color value.
        /// </param>
        /// <returns>
        ///     An array of XNA Color instances.
        /// </returns>
        public static Color[] ToColorArray(this uint[] packedValues)
        {
            Color[] result = new Color[packedValues.Length];
            for (int i = 0; i < packedValues.Length; i++)
            {
                result[i] = UINTToColor(packedValues[i]);
            }
            return result;
        }
        
        /// <summary>
        ///     Converts the array of 32-bit unsigned integers into an
        ///     array of XNA Color instances.
        /// </summary>
        /// <param name="packedValues">
        ///     An array of 32-bit unsigned integers, where each index contains
        ///     the packed color value.
        /// </param>
        /// <returns>
        ///     An array of XNA Color instances.
        /// </returns>
        public static Color32[] ToColor32Array(this uint[] packedValues)
        {
            Color32[] result = new Color32[packedValues.Length];
            for (int i = 0; i < packedValues.Length; i++)
            {
                result[i] = UINTToColor32(packedValues[i]);
            }
            return result;
        }

        /// <summary>
        ///     Converts the given 32-bit unsigned integer from being
        ///     a packed value to an XNA Color instance.
        /// </summary>
        /// <param name="color">
        ///     The 32-bit unsigned integer representing the packed value
        ///     of a color.
        /// </param>
        /// <returns>
        ///     An XNA Color instnace.
        /// </returns>
        public static Color UINTToColor(uint color)
        {

            byte r = DocColor.rgba_getr(color);
            byte g = DocColor.rgba_getg(color);
            byte b = DocColor.rgba_getb(color);
            byte a = DocColor.rgba_geta(color);

            //  Aseprite doesn't premultily alpha
            return new Color(r, g, b, a);
        }
        
        /// <summary>
        ///     Converts the given 32-bit unsigned integer from being
        ///     a packed value to an XNA Color instance.
        /// </summary>
        /// <param name="color">
        ///     The 32-bit unsigned integer representing the packed value
        ///     of a color.
        /// </param>
        /// <returns>
        ///     An XNA Color instnace.
        /// </returns>
        public static Color32 UINTToColor32(uint color)
        {

            byte r = DocColor.rgba_getr(color);
            byte g = DocColor.rgba_getg(color);
            byte b = DocColor.rgba_getb(color);
            byte a = DocColor.rgba_geta(color);

            //  Aseprite doesn't premultily alpha
            return new Color32(r, g, b, a);
        }
        /// <summary>
        ///     Converts the given R, G, B, and A color values into a
        ///     32-bit unsigned integer packed color value.
        /// </summary>
        /// <param name="r">
        ///     The Red component value of the color.
        /// </param>
        /// <param name="g">
        ///     The Green component value of the color.
        /// </param>
        /// <param name="b">
        ///     The Blue component value of the color.
        /// </param>
        /// <param name="a">
        ///     The Alpha component value of the color.
        /// </param>
        /// <returns>
        ///     A 32-bit unsigned integer representation of the color.
        /// </returns>
        public static uint BytesToPacked(byte r, byte g, byte b, byte a)
        {
            return DocColor.rgba(r, g, b, a);
        }
        
        /// <summary>
        ///     Given an <see cref="AsepriteBlendMode"/> value, returns a
        ///     <see cref="Func{T1, T2, T3, TResult}"/> that can be invoked
        ///     to perform the blending of color values.
        ///     <para>
        ///         T1 is a <see cref="uint"/> <br/>
        ///         T2 is a <see cref="uint"/> <br/>
        ///         T3 is a <see cref="int"/> <br/>
        ///         TResult is a <see cref="uint"/>
        ///     </para>
        /// </summary>
        /// <param name="mode">
        ///     The <see cref="AsepriteBlendMode"/> value that determines the blend function
        ///     to get.
        /// </param>
        /// <returns>
        ///     A <see cref="Func{T1, T2, T3, TResult}"/> instance that can be invoked to perform
        ///     the blending of color values.
        /// </returns>
        public static Func<uint, uint, int, uint> GetBlendFunction(AsepriteBlendMode mode)
        {
            switch (mode)
            {
                case AsepriteBlendMode.Normal:
                    return BlendFuncs.rgba_blender_normal;
                case AsepriteBlendMode.Multiply:
                    return BlendFuncs.rgba_blender_multiply;
                case AsepriteBlendMode.Screen:
                    return BlendFuncs.rgba_blender_screen;
                case AsepriteBlendMode.Overlay:
                    return BlendFuncs.rgba_blender_overlay;
                case AsepriteBlendMode.Darken:
                    return BlendFuncs.rgba_blender_darken;
                case AsepriteBlendMode.Lighten:
                    return BlendFuncs.rgba_blender_lighten;
                case AsepriteBlendMode.ColorDodge:
                    return BlendFuncs.rgba_blender_color_dodge;
                case AsepriteBlendMode.ColorBurn:
                    return BlendFuncs.rgba_blender_color_burn;
                case AsepriteBlendMode.HardLight:
                    return BlendFuncs.rgba_blender_hard_light;
                case AsepriteBlendMode.SoftLight:
                    return BlendFuncs.rgba_blender_soft_light;
                case AsepriteBlendMode.Difference:
                    return BlendFuncs.rgba_blender_difference;
                case AsepriteBlendMode.Exclusion:
                    return BlendFuncs.rgba_blender_exclusion;
                case AsepriteBlendMode.Hue:
                    return BlendFuncs.rgba_blender_hsl_hue;
                case AsepriteBlendMode.Saturation:
                    return BlendFuncs.rgba_blender_hsl_saturation;
                case AsepriteBlendMode.Color:
                    return BlendFuncs.rgba_blender_hsl_color;
                case AsepriteBlendMode.Luminosity:
                    return BlendFuncs.rgba_blender_hsl_luminosity;
                case AsepriteBlendMode.Addition:
                    return BlendFuncs.rgba_blender_addition;
                case AsepriteBlendMode.Subtract:
                    return BlendFuncs.rgba_blender_subtract;
                case AsepriteBlendMode.Divide:
                    return BlendFuncs.rgba_blender_divide;
                default:
                    throw new Exception("Unknown blend mode");
            }
        }
    }
}