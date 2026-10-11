using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceFont
{
    internal static LFont TFontCreate(string family, double size) => new(family, size);

    internal static LFont TFontCreate(string family, double size, string style) => new(family, size, style);

    internal static LFont TFontEmptyCreate() => new(null, 0);

    internal static CFont TFontCreate(string? family, double? size, CFontSlant slant) => new(family, size, slant);

    internal static CFontRole TFontRoleRead(LFontRole role) => (CFontRole)role;

    internal static CFont TFontRead(LSettingsPort settings, string language, CFontRole role) =>
        CFont.CFontRead(settings, language, role);
}
