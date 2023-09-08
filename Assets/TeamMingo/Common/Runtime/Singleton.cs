using UnityEngine;

namespace TeamMingo.Common.Runtime
{
  public class Singleton<T> : MonoBehaviour where T: MonoBehaviour {

    public static T Instance {
      get {
        if (!instance) {
          var gameObject = new GameObject("Singleton-" + typeof(T).Name);
          instance = gameObject.AddComponent<T>();
        }
        return instance;
      } 
    }

    internal static T instance;

    protected bool isSetup = false;

    protected virtual void Awake() {
      if (instance == null)
      {
        instance = this as T;
        if (!isSetup)
        {
          Setup();
          isSetup = true;
        }
        return;
      }
      if (this == Instance) {
        if (!isSetup)
        {
          Setup();
          isSetup = true;
        }
        return;
      }
      Destroy(this);
    }

    protected virtual void Setup()
    {
      
    }

  }

  public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {

    protected override void Awake() {
      if (instance == null)
      {
        instance = this as T;
        DontDestroyOnLoad(gameObject);
        if (!isSetup)
        {
          Setup();
          isSetup = true;
        }
        return;
      }
      if (this == Instance) {
        DontDestroyOnLoad(gameObject);
        if (!isSetup)
        {
          Setup();
          isSetup = true;
        }
        return;
      }
      Destroy(this);
    }

  }
}