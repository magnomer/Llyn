using System;
using System.Collections.Generic;
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

        LFont font = window.TWindowFontRead("Korean", LFontRole.LFontRoleGlyph);

        Assert.Equal("Noto Serif", font.LFontFamily);
        Assert.Equal([("Korean", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void WindowLocalizationRead_FakePort_ForwardsTheNormalisedName()
    {
        LSettingsPort settings = TEngineFake.TEngineCreate<LSettingsPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLocalizationRead"] = _ => "ko",
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(settings);

        Assert.Equal("ko", window.TWindowLocalizationRead());
    }

    [Fact]
    public void WindowEpithetSave_Toggle_ReachesPort()
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

        window.TWindowEpithetSave(false);
        window.TWindowEpithetSave(true);

        Assert.Equal([false, true], saved);
    }
}
