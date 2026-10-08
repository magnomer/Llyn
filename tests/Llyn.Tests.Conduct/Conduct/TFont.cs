using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFont
{
    [Fact]
    public void FontRead_LanguageAndRole_ReadsThePortFont()
    {
        List<(string, LFontRole)> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = args =>
            {
                asked.Add(((string)args![0]!, (LFontRole)args[1]!));
                return TInterfaceFont.TFontCreate("Noto Serif", 21);
            },
        });

        CFont font = TInterfaceFont.TFontRead(settings, "Korean", CFontRole.CFontRoleGlyph);

        Assert.Equal(new CFont("Noto Serif", 21, CFontSlant.CFontSlantTheme), font);
        Assert.Equal([("Korean", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void FontRead_UnsizedFont_CarriesNoSize()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = _ => TInterfaceFont.TFontCreate("Noto Serif", 0),
        });

        CFont font = TInterfaceFont.TFontRead(settings, "Korean", CFontRole.CFontRoleHeadword);

        Assert.Equal("Noto Serif", font.CFontFamily);
        Assert.Null(font.CFontSize);
    }

    [Fact]
    public void FontRead_BlankLanguage_AnswersNothingSetWithoutAsking()
    {
        List<string> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = args =>
            {
                asked.Add((string)args![0]!);
                return TInterfaceFont.TFontCreate("Noto Serif", 21);
            },
        });

        CFont font = TInterfaceFont.TFontRead(settings, " ", CFontRole.CFontRoleHeadword);

        Assert.Equal(new CFont(null, null, CFontSlant.CFontSlantTheme), font);
        Assert.Empty(asked);
    }

    [Fact]
    public void FontRead_RefusedRead_RaisesTheFailure()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = _ => throw new InvalidOperationException("no pack"),
        });

        InvalidOperationException failure = Assert.Throws<InvalidOperationException>(
            () => TInterfaceFont.TFontRead(settings, "Korean", CFontRole.CFontRoleExample));

        Assert.Equal("no pack", failure.Message);
    }

    [Fact]
    public void FontRead_SlantInAnyCaseOrUnknown_ArrivesLowerCaseOrBlank()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(new()
        {
            ["LEngineFontRead"] = args => (LFontRole)args![1]! switch
            {
                LFontRole.LFontRoleGloss => TInterfaceFont.TFontCreate("Gloss Sans", 12, "Italic"),
                LFontRole.LFontRoleExample => TInterfaceFont.TFontCreate("Example Serif", 18, "OBLIQUE"),
                _ => TInterfaceFont.TFontCreate("Head Sans", 40, "slanted"),
            },
        });

        Assert.Equal(
            [CFontSlant.CFontSlantItalic, CFontSlant.CFontSlantOblique, CFontSlant.CFontSlantTheme],
            new[] { CFontRole.CFontRoleGloss, CFontRole.CFontRoleExample, CFontRole.CFontRoleHeadword }
                .Select(role => TInterfaceFont.TFontRead(settings, "Korean", role).CFontStyle));
    }

    [Fact]
    public void FontRead_EveryEngineRole_CastsToTheSameNamedMirror()
    {
        LFontRole[] roles = Enum.GetValues<LFontRole>();

        Assert.Equal(roles.Length, Enum.GetValues<CFontRole>().Length);
        foreach (LFontRole role in roles)
        {
            Assert.Equal(role.ToString()[1..], ((CFontRole)role).ToString()[1..]);
        }
    }
}
