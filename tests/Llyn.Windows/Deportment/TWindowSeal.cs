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
    public void FootprintPostureRead_StoredGeometry_CopiesTheWindowState()
    {
        CPostureState posture = TInterfaceDeportment.TFootprintPostureRead(TInterface.TPostureStateCreate(
            TInterface.TWindowStateCreate(10, 20, 800, 600, true), linked: false, split: true, volume: 0.5));

        Assert.Equal(new CWindowState(10, 20, 800, 600, true), posture.CPostureStateWindow);
        Assert.False(posture.CPostureStateLinked);
        Assert.True(posture.CPostureStateSplit);
        Assert.Equal(0.5, posture.CPostureStateVolume);
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

    [Fact]
    public void WindowMarkdownRead_ListItem_CarriesTheEngineVerdicts()
    {
        IReadOnlyList<LMarkdownBlock> blocks = TInterface.TMarkdownParse("- **bold** item");

        CMarkdownBlock block = TInterfaceDeportment.TWindowMarkdownRead(Assert.Single(blocks));

        Assert.True(block.CMarkdownBlockListed);
        Assert.False(block.CMarkdownBlockHeaded);
        Assert.Equal("bold", block.CMarkdownBlockSpan[0].CMarkdownSpanText);
        Assert.True(block.CMarkdownBlockSpan[0].CMarkdownSpanBold);
    }
}
