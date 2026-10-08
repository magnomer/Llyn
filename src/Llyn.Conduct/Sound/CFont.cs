using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CFont(string? CFontFamily, double? CFontSize, CFontSlant CFontStyle)
{
    internal static CFont CFontRead(LSettingsPort settings, string language, CFontRole role)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(language))
        {
            return new CFont(null, null, CFontSlant.CFontSlantTheme);
        }

        LFont font = settings.LEngineFontRead(language, CFontRoleRead(role));
        return new CFont(font.LFontFace, font.LFontSized, CFontSlantRead(font.LFontStyle));
    }

    private static CFontSlant CFontSlantRead(string? style)
    {
        return style switch
        {
            "italic" => CFontSlant.CFontSlantItalic,
            "oblique" => CFontSlant.CFontSlantOblique,
            _ => CFontSlant.CFontSlantTheme,
        };
    }

    private static LFontRole CFontRoleRead(CFontRole role)
    {
        return role switch
        {
            CFontRole.CFontRoleHeadword => LFontRole.LFontRoleHeadword,
            CFontRole.CFontRoleExample => LFontRole.LFontRoleExample,
            CFontRole.CFontRoleGloss => LFontRole.LFontRoleGloss,
            CFontRole.CFontRoleGlyph => LFontRole.LFontRoleGlyph,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
        };
    }
}
