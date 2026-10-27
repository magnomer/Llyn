using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenureStale
{
    [Fact]
    public void TenureStoredRead_DraftGoneStale_AnswersNull()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        engine.TEngineWorkspaceChange(second.TWorkspaceFolder);

        Assert.Null(tenure.TTenureStoredRead());
    }

    [Fact]
    public void TenureStoredRead_LiveDraft_AnswersNullForBlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);

        Assert.Null(tenure.TTenureStoredRead());
    }

    [Fact]
    public void TenureCancel_DraftGoneStale_EndsWithoutRefusal()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = first.TWorkspaceEngineStart();
        LTenure tenure = TTenureStaleStart(engine, second);

        tenure.TTenureCancel();

        Assert.Null(tenure.TTenureRead());
    }

    [Fact]
    public void ErrandRecordingStart_DraftGoneStale_ReturnsNull()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = first.TWorkspaceEngineStart();
        LTenure tenure = TTenureStaleStart(engine, second);

        Assert.Null(tenure.TTenureRecordingStart(0, _ => { }));
    }

    [Fact]
    public void ErrandTranscriptionStart_DraftGoneStale_ReturnsNull()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = first.TWorkspaceEngineStart();
        LTenure tenure = TTenureStaleStart(engine, second);

        Assert.Null(tenure.TTenureTranscriptionStart(0, string.Empty, _ => { }));
    }

    [Fact]
    public void QuillSpeechRead_DraftGoneStale_ReturnsEmpty()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = first.TWorkspaceEngineStart();
        LTenure tenure = TTenureStaleStart(engine, second);

        (IReadOnlyList<string> names, string typed, _) = tenure.TTenureSpeechRead(string.Empty);

        Assert.Empty(names);
        Assert.Equal(string.Empty, typed);
    }

    [Fact]
    public void TenureKeptRead_DraftGoneStale_ReturnsNull()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = first.TWorkspaceEngineStart();
        LTenure tenure = TTenureStaleStart(engine, second);

        Assert.Null(tenure.TTenureKeptRead());
    }

    private static LTenure TTenureStaleStart(LEngine engine, TWorkspace second)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, entry.LEntryId);
        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);
        Assert.Throws<LRefusal>(() => tenure.TTenureRead());
        return tenure;
    }
}
