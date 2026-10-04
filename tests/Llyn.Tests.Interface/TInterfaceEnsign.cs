using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEnsign
{
    internal static Task<CEnsignSheet<TEnsignKind>> TCatalogEnsignLoad<TEnsignKind>(
        CEnvoy envoy,
        LSettingsPort settings,
        string key,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store,
        Func<TEnsignKind> read) =>
        CCatalog.LCatalogEnsignLoad(envoy, settings, key, store, read);
}
