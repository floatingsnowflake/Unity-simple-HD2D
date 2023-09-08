using System;

namespace TeamMingo.Configs.Runtime
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConfigurationAttribute : System.Attribute
  {
    public readonly string module;
    public readonly string field;

    public ConfigurationAttribute(string module, string field)
    {
      this.module = module;
      this.field = field;
    }
  }
}