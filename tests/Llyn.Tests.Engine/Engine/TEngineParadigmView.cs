using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineParadigmView
{
    [Fact]
    public void InflectionRead_SpanishVerb_AnswersCollapsedAndExpanded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        engine.TEngineAnalysisSave(false);
        LEntry entry = TParadigmEntrySave(engine, "hablar", "Spanish", "Verb");
        TParadigmFormSave(engine, entry, "9+12+17+20", "hablo");

        LParadigmView view = Assert.IsType<LParadigmView>(
            engine.TEngineInflectionRead(entry.LEntryId, false, true));

        Assert.Empty(view.LParadigmViewCollapsed.LParadigmTableHeaders);
        Assert.Equal(9, view.LParadigmViewCollapsed.LParadigmTableLines.Count);
        Assert.Equal(6, view.LParadigmViewExpanded.LParadigmTableHeaders.Count);
        Assert.Equal("Inflection.FirstSingular", view.LParadigmViewExpanded.LParadigmTableHeaders[0]);
        Assert.Equal(11, view.LParadigmViewExpanded.LParadigmTableLines.Count);
        LParadigmLine present = view.LParadigmViewExpanded.LParadigmTableLines[0];
        Assert.Equal("Inflection.Indicative", present.LParadigmLineGroup);
        Assert.Equal("Inflection.Present", present.LParadigmLineLabel);
        Assert.Equal(6, present.LParadigmLineForms.Count);
        Assert.Equal("hablo", present.LParadigmLineForms[0].LParadigmFormText);
        Assert.Equal(LParadigmStatus.LParadigmStatusText, present.LParadigmLineForms[0].LParadigmFormStatus);
        Assert.Empty(present.LParadigmLineForms[0].LParadigmFormMarks);
        Assert.Equal(LParadigmStatus.LParadigmStatusLost, present.LParadigmLineForms[1].LParadigmFormStatus);
    }

    [Fact]
    public void InflectionRead_Tener_MarksWholeRoot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "tener", "Spanish", "Verb");
        TParadigmFormSave(engine, entry, "9+13+17+20", "tuve");
        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(
            [TInterfaceInflection.TInflectionMarkCreate(1, 2)],
            Assert.Single(archive.TInflectionRead(entry.LEntryId)).LInflectionMarks);

        LParadigmView? view = engine.TEngineInflectionRead(entry.LEntryId, false, true);

        LParadigmForm form = TParadigmLineFind(
            Assert.IsType<LParadigmView>(view).LParadigmViewExpanded, "Inflection.Preterite").LParadigmLineForms[0];
        Assert.Equal("tuve", form.LParadigmFormText);
        Assert.Equal([TInterfaceInflection.TInflectionMarkCreate(0, 4)], form.LParadigmFormMarks);
        Assert.Equal(0, form.LParadigmFormSplit);
        Assert.Equal(LParadigmStatus.LParadigmStatusText, form.LParadigmFormStatus);
    }

    [Fact]
    public void InflectionRead_UnfetchedCell_AnswersPendingStatus()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "hablar", "Spanish", "Verb");

        LParadigmView? view = engine.TEngineInflectionRead(entry.LEntryId, true, true);

        IReadOnlyList<LParadigmLine> lines =
            Assert.IsType<LParadigmView>(view).LParadigmViewExpanded.LParadigmTableLines;
        Assert.Equal(LParadigmStatus.LParadigmStatusPending, lines[0].LParadigmLineForms[0].LParadigmFormStatus);
        Assert.Equal(string.Empty, lines[0].LParadigmLineForms[0].LParadigmFormText);
        LParadigmForm imperative = TParadigmLineFind(
            Assert.IsType<LParadigmView>(view).LParadigmViewExpanded, "Inflection.Affirmative").LParadigmLineForms[0];
        Assert.Equal(string.Empty, imperative.LParadigmFormText);
        Assert.Equal(LParadigmStatus.LParadigmStatusText, imperative.LParadigmFormStatus);
    }

    [Fact]
    public void InflectionRead_AnalysisOn_UsesCustomOrderAndMarks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "tener", "Spanish", "Verb");
        TParadigmFormSave(engine, entry, "9+13+17+20", "tuve");

        LParadigmView view = Assert.IsType<LParadigmView>(engine.TEngineInflectionRead(entry.LEntryId, false, true));

        Assert.True(engine.TEngineAnalysisCheck());
        Assert.Equal(
            "Inflection.FutureConditional", view.LParadigmViewCollapsed.LParadigmTableLines[0].LParadigmLineGroup);
        Assert.Equal("Inflection.Future", view.LParadigmViewExpanded.LParadigmTableLines[0].LParadigmLineGroup);
        LParadigmForm form = TParadigmLineFind(view.LParadigmViewCollapsed, "Inflection.Preterite")
            .LParadigmLineForms[0];
        Assert.Equal("tuve", form.LParadigmFormText);
        Assert.Equal([TInterfaceInflection.TInflectionMarkCreate(0, 4)], form.LParadigmFormMarks);
    }

    [Fact]
    public void InflectionRead_AnalysisOn_SplitsRootFromEnding()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "tener", "Spanish", "Verb");
        TParadigmFormSave(engine, entry, "9+12+17+20", "tengo");

        LParadigmView view = Assert.IsType<LParadigmView>(engine.TEngineInflectionRead(entry.LEntryId, false, true));

        LParadigmForm form = view.LParadigmViewExpanded.LParadigmTableLines.Single(
                line => string.Equals(line.LParadigmLineGroup, "Inflection.Indicative", StringComparison.Ordinal)
                    && string.Equals(line.LParadigmLineLabel, "Inflection.Present", StringComparison.Ordinal))
            .LParadigmLineForms[0];
        Assert.Equal("tengo", form.LParadigmFormText);
        Assert.Equal(4, form.LParadigmFormSplit);
        Assert.Equal([TInterfaceInflection.TInflectionMarkCreate(0, 4)], form.LParadigmFormMarks);
    }

    [Fact]
    public void InflectionRead_AnalysisOff_UsesDefaultOrderWithoutMarks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "tener", "Spanish", "Verb");
        TParadigmFormSave(engine, entry, "9+13+17+20", "tuve");
        engine.TEngineAnalysisSave(false);

        LParadigmView view = Assert.IsType<LParadigmView>(engine.TEngineInflectionRead(entry.LEntryId, false, true));

        Assert.False(engine.TEngineAnalysisCheck());
        Assert.Equal("Inflection.Indicative", view.LParadigmViewCollapsed.LParadigmTableLines[0].LParadigmLineGroup);
        Assert.Equal("Inflection.Indicative", view.LParadigmViewExpanded.LParadigmTableLines[0].LParadigmLineGroup);
        Assert.Equal("Inflection.Subjunctive", view.LParadigmViewExpanded.LParadigmTableLines[5].LParadigmLineGroup);
        Assert.Equal("Inflection.Imperative", view.LParadigmViewExpanded.LParadigmTableLines[9].LParadigmLineGroup);
        LParadigmForm form = TParadigmLineFind(view.LParadigmViewExpanded, "Inflection.Preterite")
            .LParadigmLineForms[0];
        Assert.Equal("tuve", form.LParadigmFormText);
        Assert.Empty(form.LParadigmFormMarks);
        Assert.All(
            view.LParadigmViewCollapsed.LParadigmTableLines.SelectMany(line => line.LParadigmLineForms),
            cell => Assert.Empty(cell.LParadigmFormMarks));
        LInflectionArchive archive = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(
            [TInterfaceInflection.TInflectionMarkCreate(1, 2)],
            Assert.Single(archive.TInflectionRead(entry.LEntryId)).LInflectionMarks);
    }

    [Fact]
    public void InflectionRead_FrenchEntry_AnswersNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "chat", "French", "Noun");

        Assert.Null(engine.TEngineInflectionRead(entry.LEntryId, false, true));
    }

    [Fact]
    public void ParadigmScan_SpanishVerb_LeavesVerbSlotsToTheView()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "tener", "Spanish", "Verb");
        TParadigmFormSave(engine, entry, "9+13+17+20", "tuve");

        Assert.NotEmpty(engine.TEngineParadigmShow(entry.LEntryId));
        Assert.Empty(engine.TEngineParadigmScan(entry.LEntryId));
    }

    [Fact]
    public void LanguageResolve_AllRegular_StillAnswersLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = TParadigmEntrySave(engine, "hablar", "Spanish", "Verb");
        LInflectionBook book = TInterface.TLanguageLoad("Spanish").LLanguageInflection!;
        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmShow(entry.LEntryId);
        engine.TEntryInflectionSave(entry.LEntryId, [.. slots.Select((slot, position) =>
            TInterfaceInflection.TInflectionCreate(
                entry.LEntryId,
                position,
                TInterfaceInflection.TInflectionBookResolve(book, "hablar", slot.LParadigmSlotCodes)!,
                null,
                slot.LParadigmSlotSpeech.LSpeechValueId,
                [.. slot.LParadigmSlotMorphologies.Select(static morphology => morphology.LMorphologyId)]))]);

        Assert.Empty(engine.TEngineParadigmShow(entry.LEntryId));
        Assert.Equal("Spanish", engine.TEngineLanguageResolve(entry.LEntryId));
    }

    private static LParadigmLine TParadigmLineFind(LParadigmTable table, string label) =>
        table.LParadigmTableLines.Single(
            line => string.Equals(line.LParadigmLineLabel, label, StringComparison.Ordinal));

    private static LEntry TParadigmEntrySave(LEngine engine, string headword, string language, string speech) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            [speech]));

    private static void TParadigmFormSave(LEngine engine, LEntry entry, string key, string text)
    {
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
    }
}
