namespace Llyn.UIDeportment;

public static class LLocalizationCatalog
{
    public static string? LLocalizationTextFind(string key)
    {
        return System.Windows.Application.Current?.TryFindResource(key) as string;
    }

    public static string LLocalizationTextRead(string key)
    {
        return LLocalizationTextFind(key) ?? key;
    }
}
