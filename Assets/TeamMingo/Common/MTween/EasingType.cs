using System;

namespace TeamMingo.MTween
{
  public enum EasingType
  {
    Linear,
    QuadIn,
    QuadOut,
    QuadInOut,
    CubicIn,
    CubicOut,
    CubicInOut,
    QuartIn,
    QuartOut,
    QuartInOut,
    QuintIn,
    QuintOut,
    QuintInOut,
    SinIn,
    SinOut,
    SinInOut,
    ExpIn,
    ExpOut,
    ExpInOut,
    CircIn,
    CircOut,
    CircInOut,
    ElastIn,
    ElastOut,
    ElastInOut,
    BackIn,
    BackOut,
    BackInOut,
    BounceIn,
    BounceOut,
    BounceInOut,
  }

  public static class EasingTypeExtensions
  {
    public static Func<float, float> ToFunc(this EasingType type)
    {
      switch (type)
      {
         case EasingType.Linear: return Easing.Linear;
         case EasingType.QuadIn: return Easing.Quad.In;
         case EasingType.QuadOut: return Easing.Quad.Out;
         case EasingType.QuadInOut: return Easing.Quad.InOut;
         case EasingType.CubicIn: return Easing.Cubic.In;
         case EasingType.CubicOut: return Easing.Cubic.Out;
         case EasingType.CubicInOut: return Easing.Cubic.InOut;
         case EasingType.QuartIn: return Easing.Quart.In;
         case EasingType.QuartOut: return Easing.Quart.Out;
         case EasingType.QuartInOut: return Easing.Quart.InOut;
         case EasingType.QuintIn: return Easing.Quint.In;
         case EasingType.QuintOut: return Easing.Quint.Out;
         case EasingType.QuintInOut: return Easing.Quint.InOut;
         case EasingType.SinIn: return Easing.Sin.In;
         case EasingType.SinOut: return Easing.Sin.Out;
         case EasingType.SinInOut: return Easing.Sin.InOut;
         case EasingType.ExpIn: return Easing.Exp.In;
         case EasingType.ExpOut: return Easing.Exp.Out;
         case EasingType.ExpInOut: return Easing.Exp.InOut;
         case EasingType.CircIn: return Easing.Circ.In;
         case EasingType.CircOut: return Easing.Circ.Out;
         case EasingType.CircInOut: return Easing.Circ.InOut;
         case EasingType.ElastIn: return Easing.Elast.In;
         case EasingType.ElastOut: return Easing.Elast.Out;
         case EasingType.ElastInOut: return Easing.Elast.InOut;
         case EasingType.BackIn: return Easing.Back.In;
         case EasingType.BackOut: return Easing.Back.Out;
         case EasingType.BackInOut: return Easing.Back.InOut;
         case EasingType.BounceIn: return Easing.Bounce.In;
         case EasingType.BounceOut: return Easing.Bounce.Out;
         case EasingType.BounceInOut: return Easing.Bounce.InOut;
         default: return Easing.Linear;
      }
    }
  }
}