using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineInflectionRegular
{
    [Fact]
    public void ParadigmUpdate_SpanishRegularForm_SetsRegular()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);

        LInflection stored = TInflectionFormSave(workspace, engine, "hablar", "Spanish", "Verb", "9+12+17+20", "hablo");

        Assert.True(stored.LInflectionRegular);
        Assert.Equal("hablo", stored.LInflectionPrediction);
        Assert.Equal([], stored.LInflectionMarks);
        Assert.Equal(TInflectionStampRead(), stored.LInflectionStamp);
    }

    [Fact]
    public void ParadigmUpdate_TenerPreterite_ClearsRegular()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);

        LInflection stored = TInflectionFormSave(workspace, engine, "tener", "Spanish", "Verb", "9+13+17+20", "tuve");

        Assert.False(stored.LInflectionRegular);
        Assert.Equal("teni", stored.LInflectionPrediction);
        Assert.Equal([TInterfaceInflection.TInflectionMarkCreate(1, 2)], stored.LInflectionMarks);
        Assert.Equal(TInflectionStampRead(), stored.LInflectionStamp);
    }

    [Fact]
    public void ParadigmUpdate_FrenchPlural_UsesParadigmRule()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);

        LInflection stored = TInflectionFormSave(workspace, engine, "chat", "French", "Noun", "4", "chats");

        Assert.Null(TInterface.TLanguageLoad("French").LLanguageInflection);
        Assert.True(stored.LInflectionRegular);
        Assert.Null(stored.LInflectionPrediction);
        Assert.Null(stored.LInflectionMarks);
        Assert.Null(stored.LInflectionStamp);
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM inflection WHERE prediction IS NULL AND marks IS NULL AND stamp IS NULL;"));
    }

    [Fact]
    public void InflectionAnalysisSave_StoredAnalysis_ReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LInflection stored = TInflectionFormSave(workspace, engine, "chat", "French", "Noun", "4", "chats");
        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TInflectionAnalysisSave(
            stored.LInflectionId,
            "chatz",
            [TInterfaceInflection.TInflectionMarkCreate(0, 1), TInterfaceInflection.TInflectionMarkCreate(3, 2)],
            "abc",
            false);
        LInflection marked = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));
        archive.TInflectionAnalysisSave(stored.LInflectionId, "chats", [], "abc", true);
        LInflection clean = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));
        archive.TInflectionAnalysisSave(stored.LInflectionId, null, null, null, false);
        LInflection uncovered = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));

        Assert.Equal("chatz", marked.LInflectionPrediction);
        Assert.Equal(
            [TInterfaceInflection.TInflectionMarkCreate(0, 1), TInterfaceInflection.TInflectionMarkCreate(3, 2)],
            marked.LInflectionMarks);
        Assert.Equal("abc", marked.LInflectionStamp);
        Assert.False(marked.LInflectionRegular);
        Assert.Equal([], clean.LInflectionMarks);
        Assert.True(clean.LInflectionRegular);
        Assert.Null(uncovered.LInflectionPrediction);
        Assert.Null(uncovered.LInflectionMarks);
        Assert.Null(uncovered.LInflectionStamp);
    }

    [Fact]
    public void InflectionStart_ChangedStamp_ReanalysesWithoutFetch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.NotFound);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        engine.TEngineMorphologySave(false);
        LInflection stored = TInflectionFormSave(workspace, engine, "tener", "Spanish", "Verb", "9+13+17+20", "tuve");
        workspace.TWorkspaceScriptRun(
            "UPDATE inflection SET stamp = 'old', marks = '', prediction = 'x', regular = 1;");

        engine.TEngineInflectionStart(stored.LInflectionEntryId);

        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);
        LInflection analysed = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));
        Assert.Equal(TInflectionStampRead(), analysed.LInflectionStamp);
        Assert.Equal("teni", analysed.LInflectionPrediction);
        Assert.Equal([TInterfaceInflection.TInflectionMarkCreate(1, 2)], analysed.LInflectionMarks);
        Assert.False(analysed.LInflectionRegular);
        Assert.Equal(0, handler.TSourceHandlerCount);
    }

    [Fact]
    public void InflectionStart_NullStampCovered_AnalysesWithoutFetch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.NotFound);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        engine.TEngineMorphologySave(false);
        LInflection stored = TInflectionFormSave(workspace, engine, "hablar", "Spanish", "Verb", "9+12+17+20", "hablo");
        workspace.TWorkspaceScriptRun(
            "UPDATE inflection SET stamp = NULL, marks = NULL, prediction = NULL, regular = 0;");

        engine.TEngineInflectionStart(stored.LInflectionEntryId);

        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);
        LInflection analysed = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));
        Assert.Equal(TInflectionStampRead(), analysed.LInflectionStamp);
        Assert.Equal("hablo", analysed.LInflectionPrediction);
        Assert.Equal([], analysed.LInflectionMarks);
        Assert.True(analysed.LInflectionRegular);
        Assert.Equal(0, handler.TSourceHandlerCount);
    }

    [Fact]
    public void InflectionStart_UncoveredRow_SkipsSecondAnalysis()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.NotFound);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        engine.TEngineMorphologySave(false);
        LInflection stored = TInflectionFormSave(workspace, engine, "pon", "Spanish", "Verb", "9+12+17+20", "pongo");
        workspace.TWorkspaceScriptRun("UPDATE inflection SET stamp = NULL, regular = 1;");
        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);

        engine.TEngineInflectionStart(stored.LInflectionEntryId);
        LInflection analysed = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));
        workspace.TWorkspaceScriptRun("UPDATE inflection SET regular = 1;");
        engine.TEngineInflectionStart(stored.LInflectionEntryId);
        LInflection kept = Assert.Single(archive.TInflectionRead(stored.LInflectionEntryId));

        Assert.Equal(TInflectionStampRead(), stored.LInflectionStamp);
        Assert.Null(stored.LInflectionPrediction);
        Assert.Null(stored.LInflectionMarks);
        Assert.Equal(TInflectionStampRead(), analysed.LInflectionStamp);
        Assert.Null(analysed.LInflectionMarks);
        Assert.False(analysed.LInflectionRegular);
        Assert.Equal(TInflectionStampRead(), kept.LInflectionStamp);
        Assert.True(kept.LInflectionRegular);
        Assert.Equal(0, handler.TSourceHandlerCount);
    }

    private static string TInflectionStampRead() =>
        TInterface.TLanguageLoad("Spanish").LLanguageInflection!.LInflectionBookStamp;

    private static LInflection TInflectionFormSave(
        TWorkspace workspace, LEngine engine, string headword, string language, string speech, string key, string text)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            [speech]));
        LParadigmSlot slot = engine.TEngineParadigmShow(entry.LEntryId)
            .Single(row => string.Equals(row.LParadigmSlotKey, key, StringComparison.Ordinal));
        engine.TEntryInflectionSave(entry.LEntryId, [
            TInterfaceInflection.TInflectionCreate(
                entry.LEntryId,
                0,
                text,
                null,
                slot.LParadigmSlotSpeech.LSpeechValueId,
                [.. slot.LParadigmSlotMorphologies.Select(static morphology => morphology.LMorphologyId)]),
        ]);

        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);
        return Assert.Single(archive.TInflectionRead(entry.LEntryId));
    }
}
