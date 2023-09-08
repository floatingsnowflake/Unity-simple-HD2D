using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamMingo.MTween {

  public class TweenProps : Dictionary<string, object> {

    public static TweenProps FromParams(params object[] args) {
      if (args.Length % 2 != 0) {
        throw TweenException.ArgsMustBeEven();
      }
      var props = new TweenProps();
      int i = 0;
			while(i < args.Length - 1) {
				props.Add(args[i].ToString(), args[i+1]);
				i += 2;
			}
			return props;
    }

    public static TweenProps Scale(Vector3 scale)
    {
      return FromParams("scale", scale);
    }

  }

}