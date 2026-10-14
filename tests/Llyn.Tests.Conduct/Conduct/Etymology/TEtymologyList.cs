using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymologyList
{
    [Fact]
    public void EtymologyRelationRead_Workspace_AnswersBuiltinsThenCustomThenAddition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long custom = engine.TEngineRelationCreate("inherited").LAncestorRelationId;
        CEtymology etymology = TEtymologyListPrepare(engine, TEnvoyFake.TEnvoyCreate(false, []));

        IReadOnlyList<CAncestorChoice> rows = etymology.CEtymologyRead().CEtymologyStateRelations;

        Assert.Equal(
            ["Relation.Borrowed", "Relation.Compound", "Relation.Derived", "Relation.Calque", "Relation.Blend"],
            rows.Take(5).Select(static row => row.CAncestorChoiceKey));
        Assert.All(rows.Take(5), static row => Assert.False(row.CAncestorChoiceCustom));
        Assert.Equal(new CAncestorChoice(custom, string.Empty, "inherited", true, false), rows[5]);
        Assert.Equal(new CAncestorChoice(null, "Relation.Add", string.Empty, false, true), rows[^1]);
        Assert.Equal(7, rows.Count);
    }

    [Fact]
    public void EtymologyLanguageRead_Tongue_AnswersLanguagesThenTonguesThenAddition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long? tongue = engine.TEngineTongueCreate("Proto-Germanic").LAncestorLanguageTongue;
        CEtymology etymology = TEtymologyListPrepare(engine, TEnvoyFake.TEnvoyCreate(false, []));

        IReadOnlyList<CAncestorChoice> rows = etymology.CEtymologyRead().CEtymologyStateLanguages;

        Assert.Equal(
            engine.TEngineLanguageRead(),
            rows.SkipLast(2).Select(static row => row.CAncestorChoiceName));
        Assert.All(rows.SkipLast(2), static row => Assert.Null(row.CAncestorChoiceId));
        Assert.Equal(new CAncestorChoice(tongue, string.Empty, "Proto-Germanic", true, false), rows[^2]);
        Assert.Equal(new CAncestorChoice(null, "Tongue.Add", string.Empty, false, true), rows[^1]);
    }

    [Fact]
    public void EtymologyRelationSet_Addition_CreatesTheRelationAndSetsItOnTheBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCoinageCreate(" inherited ", null, asked);
        CEtymology etymology = TEtymologyListPrepare(engine, envoy);
        long first = TEtymology.TEtymologyBoxCreate(etymology);

        etymology.CEtymologyRelationSet(first, etymology.CEtymologyRead().CEtymologyStateRelations[^1]);

        LAncestorRelation created = Assert.Single(
            engine.TEngineRelationRead(), static relation => relation.LAncestorRelationName == "inherited");
        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal(created.LAncestorRelationId, row.CAncestorRelation);
        Assert.Equal(["Coinage:Coinage.Relation"], asked);
    }

    [Fact]
    public void EtymologyRelationSet_AdditionDeclined_CreatesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CEtymology etymology = TEtymologyListPrepare(engine, TEnvoyFake.TEnvoyCoinageCreate(null, null, asked));
        long first = TEtymology.TEtymologyBoxCreate(etymology);
        int before = engine.TEngineRelationRead().Count;

        etymology.CEtymologyRelationSet(first, etymology.CEtymologyRead().CEtymologyStateRelations[^1]);

        Assert.Equal(before, engine.TEngineRelationRead().Count);
        Assert.Null(Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors).CAncestorRelation);
        Assert.Equal(["Coinage:Coinage.Relation"], asked);
    }

    [Fact]
    public void EtymologyLanguageSet_Addition_CreatesTheTongueAndSetsItOnTheBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCoinageCreate("Proto-Germanic", null, asked);
        CEtymology etymology = TEtymologyListPrepare(engine, envoy);
        long first = TEtymology.TEtymologyBoxCreate(etymology);

        etymology.CEtymologyLanguageSet(first, etymology.CEtymologyRead().CEtymologyStateLanguages[^1]);

        Assert.Contains(
            etymology.CEtymologyRead().CEtymologyStateLanguages,
            static row => row.CAncestorChoiceName == "Proto-Germanic" && row.CAncestorChoiceCustom);
        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal("Proto-Germanic", row.CAncestorLanguage);
        Assert.Equal(["Coinage:Coinage.Tongue"], asked);
    }

    [Fact]
    public void EtymologyRelationDelete_UsedCustom_ConfirmsWithTheCountAndClearsEveryBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long custom = engine.TEngineRelationCreate("inherited").LAncestorRelationId;
        CAncestorChoice chosen = new(custom, string.Empty, "inherited", true, false);
        List<string> asked = [];
        long water = TEtymology.TEtymologySave(engine, "water", "English");
        TEditorFixture editor = TEtymology.TEtymologyEditorPrepare(
            engine, water, TInterfaceEditor.TEditorCreate(engine, TEnvoyFake.TEnvoyCreate(true, asked)));
        CEtymology etymology = editor.TEditorFixtureEtymology;
        long first = TEtymology.TEtymologyBoxCreate(etymology);
        etymology.CEtymologyRelationSet(first, chosen);
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();
        editor.TEditorFixtureSave();
        etymology.CEtymologyRelationSet(TEtymology.TEtymologyBoxCreate(etymology), chosen);

        etymology.CEtymologyRelationDelete(
            etymology.CEtymologyRead().CEtymologyStateRelations.Single(static row => row.CAncestorChoiceCustom));

        Assert.Equal(["Relation.DetachConfirm"], asked);
        Assert.DoesNotContain(engine.TEngineRelationRead(), relation => relation.LAncestorRelationId == custom);
        Assert.Equal(2, etymology.CEtymologyRead().CEtymologyStateAncestors.Count);
        IReadOnlyList<CAncestor> boxes = etymology.CEtymologyRead().CEtymologyStateAncestors;
        Assert.All(boxes, static row => Assert.Null(row.CAncestorRelation));
        Assert.Null(Assert.Single(TEtymologyStoredRead(engine, water)).CAncestorRelation);
    }

    [Fact]
    public void EtymologyRelationDelete_Declined_KeepsTheRelation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRelationCreate("inherited");
        List<string> asked = [];
        CEtymology etymology = TEtymologyListPrepare(engine, TEnvoyFake.TEnvoyCreate(false, asked));

        etymology.CEtymologyRelationDelete(
            etymology.CEtymologyRead().CEtymologyStateRelations.Single(static row => row.CAncestorChoiceCustom));

        Assert.Equal(["Relation.DeleteConfirm"], asked);
        Assert.Contains(engine.TEngineRelationRead(), static relation => relation.LAncestorRelationName == "inherited");
    }

    [Fact]
    public void EtymologyRelationDelete_Builtin_ShowsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CEtymology etymology = TEtymologyListPrepare(engine, TEnvoyFake.TEnvoyCreate(true, asked));
        int before = engine.TEngineRelationRead().Count;

        etymology.CEtymologyRelationDelete(etymology.CEtymologyRead().CEtymologyStateRelations[0]);
        etymology.CEtymologyRelationDelete(etymology.CEtymologyRead().CEtymologyStateRelations[^1]);

        Assert.Empty(asked);
        Assert.Equal(before, engine.TEngineRelationRead().Count);
    }

    [Fact]
    public void EtymologyLanguageDelete_UsedTongue_ConfirmsWithTheCountAndClearsEveryBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTongueCreate("Proto-Germanic");
        List<string> asked = [];
        long water = TEtymology.TEtymologySave(engine, "water", "English");
        TEditorFixture editor = TEtymology.TEtymologyEditorPrepare(
            engine, water, TInterfaceEditor.TEditorCreate(engine, TEnvoyFake.TEnvoyCreate(true, asked)));
        CEtymology etymology = editor.TEditorFixtureEtymology;
        IReadOnlyList<CAncestorChoice> languages = etymology.CEtymologyRead().CEtymologyStateLanguages;
        CAncestorChoice tongue = languages.Single(static row => row.CAncestorChoiceCustom);
        etymology.CEtymologyLanguageSet(TEtymology.TEtymologyBoxCreate(etymology), tongue);
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();
        editor.TEditorFixtureSave();

        etymology.CEtymologyLanguageDelete(tongue);

        Assert.Equal(["Tongue.DetachConfirm"], asked);
        IReadOnlyList<CAncestorChoice> kept = etymology.CEtymologyRead().CEtymologyStateLanguages;
        Assert.DoesNotContain(kept, static row => row.CAncestorChoiceCustom);
        Assert.Empty(Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors).CAncestorLanguage);
        Assert.Equal(string.Empty, Assert.Single(TEtymologyStoredRead(engine, water)).CAncestorLanguage);
    }

    [Fact]
    public void EtymologyLanguageDelete_UnusedTongue_ConfirmsAndDeletes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTongueCreate("Proto-Germanic");
        List<string> asked = [];
        CEtymology etymology = TEtymologyListPrepare(engine, TEnvoyFake.TEnvoyCreate(true, asked));

        etymology.CEtymologyLanguageDelete(
            etymology.CEtymologyRead().CEtymologyStateLanguages.Single(static row => row.CAncestorChoiceCustom));

        Assert.Equal(["Tongue.DeleteConfirm"], asked);
        IReadOnlyList<CAncestorChoice> kept = etymology.CEtymologyRead().CEtymologyStateLanguages;
        Assert.DoesNotContain(kept, static row => row.CAncestorChoiceCustom);
    }

    internal static IReadOnlyList<CAncestor> TEtymologyStoredRead(LEngine engine, long entry)
    {
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(entry, engine.TEngineEntryLoad(entry)!);
        return display.CDisplayCard.CDisplayEtymologyRead().CEtymologyStateAncestors;
    }

    private static CEtymology TEtymologyListPrepare(LEngine engine, CEnvoy envoy)
    {
        long water = TEtymology.TEtymologySave(engine, "water", "English");
        return TEtymology.TEtymologyEditorPrepare(engine, water, TInterfaceEditor.TEditorCreate(engine, envoy))
            .TEditorFixtureEtymology;
    }
}
