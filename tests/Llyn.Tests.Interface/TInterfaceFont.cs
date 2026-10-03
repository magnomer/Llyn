using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceFont
{
    internal static LFont TFontCreate(string family, double size) => new(family, size);

    internal static LFont TFontCreate(string family, double size, string style) => new(family, size, style);

    internal static CFont TCatalogFontRead(LSettingsPort settings, string language, CFontRole role) =>
        CCatalog.LCatalogFontRead(settings, language, role);
}
