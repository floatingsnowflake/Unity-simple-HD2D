namespace TeamMingo.Common.Runtime.Extensions
{
  public static class StringExtensions
  {
    public static bool IsNullOrWhitespace(this string str)
    {
      if (!string.IsNullOrEmpty(str))
      {
        for (int index = 0; index < str.Length; ++index)
        {
          if (!char.IsWhiteSpace(str[index]))
            return false;
        }
      }
      return true;
    }
  }
}