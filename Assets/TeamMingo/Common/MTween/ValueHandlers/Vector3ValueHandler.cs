using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamMingo.MTween {

  public class Vector3ValueHandler : ValueHandler<Vector3> {

    public override Type HandleType { get { return typeof(Vector3); } }

    public override Vector3 Plus(Vector3 a, Vector3 b) {
      return a + b;
    }

    public override Vector3 Subtract(Vector3 a, Vector3 b) {
      return a - b;
    }

    public override Vector3 Easing(Vector3 start, Vector3 end, float easingValue) {
      return start + (end - start) * easingValue;
    }
    
  }

}