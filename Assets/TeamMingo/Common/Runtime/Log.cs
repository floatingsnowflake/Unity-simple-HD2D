using UnityEngine;

namespace TeamMingo.Common.Runtime
{
  public class Log
  {
    public static Log Get<T>()
    {
      return new Log(typeof(T).Name);
    }

    public static Log Get(MonoBehaviour behaviour)
    {
      return new Log($"{behaviour.gameObject.name}.{behaviour.GetType().Name}");
    }

    public readonly string Tag;

    private Log(string tag)
    {
      Tag = tag;
    }

    public void D(object message)
    {
      Debug.Log($"{Time.time} [{Tag}] {message}");
    }
  }
}