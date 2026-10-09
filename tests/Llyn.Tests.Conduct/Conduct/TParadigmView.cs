using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TParadigmView
{
    [Fact]
    public void DisplayParadigmRead_ViewAnswered_MapsMarksAndHeaders()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LParadigmView view = new(
            TInterfaceInflection.TParadigmTableCreate(
                [],
                [TInterfaceInflection.TParadigmLineCreate(
                    "Inflection.Indicative", "Inflection.Present", [TParadigmFormCreate("tuve", 1, 2)])]),
            TInterfaceInflection.TParadigmTableCreate(
                ["Inflection.FirstSingular"],
                [TInterfaceInflection.TParadigmLineCreate(
                    string.Empty, string.Empty, [TParadigmFormCreate("hablo", 0, 0)])]));
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TParadigmAnswersCreate(engine, _ => view));
        List<string> asked = [];
        CWing wing = TParadigmWingOpen(engine, atelier, asked);

        CParadigmView? shown = wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead().CLecternParadigmView;

        Assert.NotNull(shown);
        CParadigmLine collapsed = Assert.Single(shown.CParadigmViewCollapsed.CParadigmTableLines);
        Assert.Equal("Inflection.Indicative", collapsed.CParadigmLineGroup);
        Assert.Equal("Inflection.Present", collapsed.CParadigmLineLabel);
        CParadigmForm form = Assert.Single(collapsed.CParadigmLineForms);
        Assert.Equal("tuve", form.CParadigmFormText);
        Assert.Null(form.CParadigmFormTip);
        Assert.Equal([new CParadigmMark(1, 2)], form.CParadigmFormMarks);
        Assert.Equal(["Inflection.FirstSingular"], shown.CParadigmViewExpanded.CParadigmTableHeaders);
        Assert.DoesNotContain("Display.ParadigmReadFailed", asked);
    }

    [Fact]
    public void SoundingParadigmRead_LostCell_AnswersHeldTip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LParadigmTable table = new(
            [],
            [TInterfaceInflection.TParadigmLineCreate(
                string.Empty,
                string.Empty,
                [TInterfaceInflection.TParadigmFormCreate(string.Empty, [], LParadigmStatus.LParadigmStatusLost)])]);
        LParadigmView view = new(table, table);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], [])).LEntryId);
        CSounding sounding = TInterfaceConductSound.TSoundingCreate(
            editor.TEditorFixtureDesk,
            TInterfaceConduct.TPhonologyBundleCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineParadigmScan"] = _ => (IReadOnlyList<LParadigmRow>)[],
                ["LEngineLanguageResolve"] = _ => "Latin",
                ["LEngineInflectionCheck"] = _ => false,
                ["LEngineInflectionRead"] = _ => view,
            }),
            TEnvoyFake.TEnvoyCreate(false, []));

        CParadigmView? shown = sounding.CSoundingParadigmRead().CLecternParadigmView;

        Assert.NotNull(shown);
        CParadigmForm form = Assert.Single(
            Assert.Single(shown.CParadigmViewCollapsed.CParadigmTableLines).CParadigmLineForms);
        Assert.Equal(new CParadigmForm("…", [], "Paradigm.Held"), form);
    }

    [Fact]
    public void DisplayParadigmRead_ViewFails_AnswersNullViewAndNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine, TParadigmAnswersCreate(engine, _ => throw new InvalidOperationException("refused")));
        List<string> asked = [];
        CWing wing = TParadigmWingOpen(engine, atelier, asked);
        asked.Clear();

        CLecternParadigm paradigm = wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.Null(paradigm.CLecternParadigmView);
        Assert.Contains("Display.ParadigmReadFailed", asked);
    }

    private static LParadigmForm TParadigmFormCreate(string text, int offset, int length) =>
        new(
            text,
            length == 0 ? [] : [TInterfaceInflection.TInflectionMarkCreate(offset, length)],
            LParadigmStatus.LParadigmStatusText);

    private static Dictionary<string, Func<object?[]?, object?>> TParadigmAnswersCreate(
        LEngine engine, Func<object?[]?, object?> read) =>
        new()
        {
            ["LEngineEntryLoad"] = args => engine.TEngineEntryLoad((long)args![0]!),
            ["LEngineParadigmScan"] = _ => (IReadOnlyList<LParadigmRow>)[],
            ["LEngineLanguageResolve"] = _ => "Latin",
            ["LEngineInflectionCheck"] = _ => false,
            ["LEngineInflectionRead"] = read,
        };

    private static CWing TParadigmWingOpen(LEngine engine, CAtelier atelier, List<string> asked)
    {
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        wing.CWingEntryOpen(water.LEntryId);
        return wing;
    }
}
