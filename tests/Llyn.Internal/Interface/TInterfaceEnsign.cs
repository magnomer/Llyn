using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEnsign
{
    internal static Task<CEnsignSheet<TEnsignKind>> TCatalogEnsignLoad<TEnsignKind>(
        LSettingsPort settings,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store,
        Func<TEnsignKind> read) =>
        CCatalog.LCatalogEnsignLoad(settings, store, read);
}
