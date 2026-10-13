namespace Llyn.Conduct;

public sealed record CScheme(string CSchemeName, bool CSchemeTaken)
{
    public string CSchemeKey => CSchemeKeyRead(CSchemeName);

    public static string CSchemeKeyRead(string name)
    {
        return string.Concat("Scheme.", name);
    }
}
