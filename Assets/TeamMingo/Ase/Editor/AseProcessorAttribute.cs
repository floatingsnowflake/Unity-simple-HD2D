using System;

namespace TeamMingo.Ase.Editor
{
  [AttributeUsage(AttributeTargets.Class)]
  public class AseProcessorAttribute : System.Attribute
  {
    public readonly Type type;

    public AseProcessorAttribute(Type type)
    {
      this.type = type;
    }
  }
}