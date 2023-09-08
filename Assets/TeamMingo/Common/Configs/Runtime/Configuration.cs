using UnityEngine;

namespace TeamMingo.Configs.Runtime
{
  public class Configuration : ScriptableObject
  {
    public static readonly string Path = "TeamMingo Configuration";
    
    private static Configuration _instance;
    
    public static Configuration Get()
    {
      if (!_instance)
      {
        _instance = Resources.Load<Configuration>(Path);
      }

      return _instance;
    }
    
    public ScriptableObject i18nSettings;
    public ScriptableObject screenTransitionSettings;
    public ScriptableObject inventoryDatabase;
  }
}