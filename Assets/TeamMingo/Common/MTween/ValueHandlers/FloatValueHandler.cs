using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamMingo.MTween {

  public class FloatValueHandler : ValueHandler<float> {

    public override Type HandleType { get { return typeof(float); } }

    public override float Plus(float a, float b) {
      return ValueUtils.Plus(a, b);
    }

    public override float Subtract(float a, float b) {
      return ValueUtils.Subtract(a, b);
    }

    public override float Easing(float start, float end, float easingValue) {
      return ValueUtils.Easing(start, end, easingValue);
    }
  }

}