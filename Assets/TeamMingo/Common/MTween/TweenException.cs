using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamMingo.MTween {

  public class TweenException : UnityException {

    public static TweenException BridgeKeyHasBeenUsed(string key, Type type) {
      return new TweenException(String.Format("This key has been used for the type of {1}: {0}", key, type.FullName));
    }

    public static TweenException InvalidKey(string key, Type type) {
      return new TweenException(String.Format("This key is invalid for the type of {1}: {0}", key, type.FullName));
    }

    public static TweenException NotATweenTarget(Type type) {
      return new TweenException(String.Format("This object is not a tween target: {0}", type.FullName));
    }

    public static TweenException ArgsMustBeEven() {
      return new TweenException("T.p(...) requires an even number of arguments!");
    }

    public static TweenException ExistsValueHandlerForTheType(Type type) {
      return new TweenException(string.Format("There is an ValueHandler for the type: {0}", type));
    }

    public static TweenException UnhandledValueType(Type type) {
      return new TweenException(string.Format("This type of value can not be handled: {0}", type));
    }

    public TweenException(string message): base(message) {}

  }

}