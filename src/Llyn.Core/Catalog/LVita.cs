using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LVita(
    string LVitaName,
    bool LVitaNamed,
    string LVitaWork,
    string LVitaTally,
    IReadOnlyList<LFellow> LVitaFellows,
    IReadOnlyList<LUsage> LVitaUsages)
{
    private static readonly LCatalogAuthor LVitaNobody = new(new LAuthor(0, string.Empty), 0, 0);

    public bool LVitaFellowShown => LVitaFellows.Count > 0;

    public bool LVitaUsageShown => LVitaUsages.Count > 0;

    public static LVita LVitaCreate(
        LCatalogAuthor? row,
        IReadOnlyList<LFellow> fellows,
        IReadOnlyList<LUsage> usages,
        Func<string, string> localize)
    {
        ArgumentNullException.ThrowIfNull(fellows);
        ArgumentNullException.ThrowIfNull(usages);
        ArgumentNullException.ThrowIfNull(localize);

        LCatalogAuthor shown = row ?? LVitaNobody;
        bool named = shown.LCatalogAuthorStored.LAuthorNamed;
        return new LVita(
            named ? shown.LCatalogAuthorName : localize("Guild.Unnamed"),
            named,
            LVitaWorkFormat(shown.LCatalogAuthorWork, localize),
            LReference.LReferenceUsageFormat(shown.LCatalogAuthorUsage, localize),
            fellows,
            usages);
    }

    public static string LVitaWorkFormat(int count, Func<string, string> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        return count switch
        {
            0 => localize("Guild.WorkNone"),
            1 => localize("Guild.WorkOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} {localize("Guild.WorkMany")}",
        };
    }
}
