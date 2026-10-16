using System;
using System.Globalization;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineStamp
{
    [Fact]
    public void EngineStampRead_StoredEntry_WordsBothTimes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));

        (bool stored, string added, string updated) = engine.TEngineStampRead(water.LEntryId);

        Assert.True(stored);
        Assert.NotEmpty(added);
        Assert.NotEmpty(updated);
    }

    [Fact]
    public void StampRead_KoreanMachineCulture_WordsInterfaceLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        CultureInfo machine = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ko-KR");
        string added;
        try
        {
            (_, added, _) = engine.TEngineStampRead(water.LEntryId);
        }
        finally
        {
            CultureInfo.CurrentCulture = machine;
        }

        DateTimeOffset stored = DateTimeOffset.Parse(
            water.LEntryAddedUtc!, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        Assert.Equal(stored.ToLocalTime().ToString("g", CultureInfo.GetCultureInfo("en")), added);
    }

    [Fact]
    public void EngineStampRead_UnknownEntry_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal((false, string.Empty, string.Empty), engine.TEngineStampRead(987654));
    }

    [Fact]
    public void EngineStampFormat_UnreadableText_ReturnsEmpty()
    {
        Assert.Empty(TInterface.TEngineStampFormat("not a moment"));
        Assert.Empty(TInterface.TEngineStampFormat(null));
    }
}
