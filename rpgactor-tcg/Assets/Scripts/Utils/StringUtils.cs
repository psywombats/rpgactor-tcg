using System.Linq;

public static class StringUtils
{
    public static string ToAscii(this string str)
    {
        return new string(str.Where(c => c < 256).ToArray());
    }
    
    public static string StripNonAlpha(this string str)
    {
        return new string(str.Where(c => (c >= 65 && c <= 90) || (c >= 97 && c <= 122)).ToArray());
    }
}