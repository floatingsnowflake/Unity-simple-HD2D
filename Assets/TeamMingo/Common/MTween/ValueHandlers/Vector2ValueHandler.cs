using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamMingo.MTween {

  public class Vector2ValueHandler : ValueHandler<Vector2> {

    public override Type HandleType { get { return typeof(Vector2); } }

    public override Vector2 Plus(Vector2 a, Vector2 b) {
      return a + b;
    }

    public override Vector2 Subtract(Vector2 a, Vector2 b) {
      return a - b;
    }

    public override Vector2 Easing(Vector2 start, Vector2 end, float easingValue) {
      return start + (end - start) * easingValue;
    }
    
  }

}