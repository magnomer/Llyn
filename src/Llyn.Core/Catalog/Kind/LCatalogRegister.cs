using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogRegister(
    LRegister LCatalogRegisterStored,
    int LCatalogRegisterUsage,
    bool LCatalogRegisterChosen = false)
{
    private static readonly Dictionary<string, string> LCatalogRegisterIcons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["archaic"] = "register/archaic",
        ["epic"] = "register/epic",
        ["formal"] = "register/formal",
        ["impolite"] = "register/impolite",
        ["informal"] = "register/informal",
        ["poetic"] = "register/poetic",
        ["polite"] = "register/polite",
    };

    public string LCatalogRegisterIcon => LCatalogRegisterIcons.GetValueOrDefault(
        LCatalogRegisterStored.LRegisterName.LStateValueShow().Trim(),
        "register");

    public static LCatalogRegister LCatalogRegisterCreate(LRegister register, int usage)
    {
        ArgumentNullException.ThrowIfNull(register);

        return new LCatalogRegister(register, usage);
    }

    public static IReadOnlyList<LCatalogRegister> LCatalogRegisterSort(
        IReadOnlyList<LCatalogRegister> rows,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. rows
                .OrderByDescending(
                    row => row.LCatalogRegisterStored.LRegisterName.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderUsage => [.. rows
                .OrderByDescending(row => row.LCatalogRegisterUsage)],
            _ => [.. rows
                .OrderBy(
                    row => row.LCatalogRegisterStored.LRegisterName.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)],
        };
    }

    public bool LCatalogRegisterMatch(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query.Length == 0
            || LCatalog.LCatalogTextMatch(LCatalogRegisterStored.LRegisterName.LStateValueShow(), query);
    }
}
