using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowPort
{
    [Fact]
    public void WindowFontRead_LanguageAndRole_ForwardsToPort()
    {
        List<(string, LFontRole)> asked = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFontRead"] = args =>
            {
                asked.Add(((string)args![0]!, (LFontRole)args[1]!));
                return TInterfaceDeportment.TFontCreate("Noto Serif", 21);
            },
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(settings);

        CFont font = window.TWindowFontRead("Korean", CFontRole.CFontRoleGlyph);

        Assert.Equal("Noto Serif", font.CFontFamily);
        Assert.Equal(21, font.CFontSize);
        Assert.Equal([("Korean", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void WorkspaceLocalizationRead_FakePort_ForwardsTheNormalisedName()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLocalizationRead"] = _ => "ko",
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(settings);

        Assert.Equal("ko", window.TWorkspaceLocalizationRead());
    }

    [Fact]
    public void WorkspaceEpithetSave_Toggle_ReachesPort()
    {
        List<bool> saved = [];
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineEpithetSave"] = args =>
            {
                saved.Add((bool)args![0]!);
                return null;
            },
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(settings);

        window.TWorkspaceEpithetSave(false);
        window.TWorkspaceEpithetSave(true);

        Assert.Equal([false, true], saved);
    }
}
