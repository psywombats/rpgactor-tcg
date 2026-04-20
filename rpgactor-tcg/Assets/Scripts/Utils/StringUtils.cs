using System.Linq;

public static class StringUtils
{
    public static string ToAscii(this string str)
    {
        return new string(str.Where(c => c < 256).ToArray());
    }
}