using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowSeal
{
    [Fact]
    public void WindowFontRead_EveryEngineRole_CastsToTheSameNamedMirror()
    {
        LFontRole[] roles = Enum.GetValues<LFontRole>();

        Assert.Equal(roles.Length, Enum.GetValues<CFontRole>().Length);
        foreach (LFontRole role in roles)
        {
            Assert.Equal(role.ToString()[1..], ((CFontRole)role).ToString()[1..]);
        }
    }

    [Fact]
    public void SoundingFontRead_UnsizedFont_CarriesNoSize()
    {
        CFont font = TInterfaceDeportment.TSoundingFontRead(TInterfaceDeportment.TFontCreate("Noto Serif", 0));

        Assert.Equal("Noto Serif", font.CFontFamily);
        Assert.Null(font.CFontSize);
    }

    [Fact]
    public void WorkspaceSettingsRead_TwoLookupsOn_CountsThemOnline()
    {
        CSettings settings = TInterfaceDeportment.TWorkspaceSettingsRead(
            TInterface.TSettingsCreate("ko", respelled: true, frequency: true, morphology: true, epithet: false));

        Assert.Equal("ko", settings.CSettingsLocalization);
        Assert.True(settings.CSettingsRespelled);
        Assert.False(settings.CSettingsEpithet);
        Assert.Equal(2, settings.CSettingsOnline);
    }
}
