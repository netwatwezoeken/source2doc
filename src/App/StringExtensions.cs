namespace App;

public static class StringExtensions
{
    public static ClassIdentifier ToClassIdentifier(this string input)
    {
        var split = input.Split('.');
        return split.Length == 1 ? 
            new ClassIdentifier("", split[0]) : 
            new ClassIdentifier(string.Join('.',split.SkipLast(1)), split[^1]);
    }
}