using System.Collections.Generic;
using System.IO;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupAncestor
{
    [Fact]
    public void MarkupExport_Map_WritesEachBoxByName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TMarkupAncestorSave(engine);

        LMarkupEntry entry = TMarkupAncestorExport(engine, workspace, entryId, out _);

        Assert.Equal(
            [
                "1||0|rinnan||English||borrowed",
                "2||1||*rinnaną|Proto-Germanic|to run|echo",
                "3|2|0||*rei-||to flow|",
            ],
            entry.LMarkupEntryAncestor.Select(static box => string.Join(
                "|",
                box.LMarkupAncestorKey,
                box.LMarkupAncestorChild,
                box.LMarkupAncestorPosition,
                box.LMarkupAncestorHeadword,
                box.LMarkupAncestorText,
                box.LMarkupAncestorLanguage,
                box.LMarkupAncestorGloss,
                box.LMarkupAncestorRelation)));
    }

    [Fact]
    public void MarkupImport_ExportedMap_StoresSameTree()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TMarkupAncestorSave(engine);
        TMarkupAncestorExport(engine, workspace, entryId, out string text);
        string path = workspace.TWorkspaceMarkupSave(text.Replace(">run<", ">ran<", System.StringComparison.Ordinal));

        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        IReadOnlyList<LAncestorDraft> stored = TMarkupAncestorRead(engine, entryId);
        IReadOnlyList<LAncestorDraft> imported =
            TMarkupAncestorRead(engine, Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId);
        Assert.Equal(TAncestorShapeRead(stored), TAncestorShapeRead(imported));
    }

    [Fact]
    public void MarkupImport_OldEtymology_LoadsEntryWithOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string path = workspace.TWorkspaceMarkupSave(
            """
            <llyn>
              <entry>
                <headword>run</headword>
                <language>English</language>
                <etymon><headword>rinnan</headword><language>English</language></etymon>
                <etymology><text>From rinnan.</text></etymology>
              </entry>
            </llyn>
            """);

        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Equal(
            ["<etymon>", "<etymology>"],
            outcome.TMarkupOutcomeOmission.Select(static omission => omission.LMarkupOmissionText));
        LEntry stored = Assert.Single(outcome.TMarkupOutcomeEntry);
        Assert.Equal("run", stored.LEntryHeadword);
        Assert.Empty(TMarkupAncestorRead(engine, stored.LEntryId));
    }

    [Fact]
    public void MarkupImport_UnknownLink_KeepsHeadwordAsText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string path = workspace.TWorkspaceMarkupSave(
            """
            <llyn>
              <entry>
                <headword>run</headword>
                <language>English</language>
                <ancestor>
                  <key>1</key>
                  <headword>rinnan</headword>
                  <language>English</language>
                </ancestor>
              </entry>
            </llyn>
            """);

        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Equal("ancestor \"rinnan\"", Assert.Single(outcome.TMarkupOutcomeOmission).LMarkupOmissionText);
        LAncestorDraft box = Assert.Single(
            TMarkupAncestorRead(engine, Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        Assert.Equal(("rinnan", null), (box.LAncestorDraftText, box.LAncestorDraftTarget));
    }

    private static long TMarkupAncestorSave(LEngine engine)
    {
        long source = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "rinnan", "English", string.Empty, string.Empty, [], [])).LEntryId;
        long borrowed = engine.TEngineRelationRead()
            .First(static relation => relation.LAncestorRelationName == "borrowed").LAncestorRelationId;
        long echo = engine.TEngineRelationCreate("echo").LAncestorRelationId;
        long? tongue = engine.TEngineTongueCreate("Proto-Germanic").LAncestorLanguageTongue;
        LEntryDraft draft = TInterface.TEntryDraftCreate("run", "English", string.Empty, string.Empty, [], []) with
        {
            LEntryDraftAncestors =
            [
                TInterface.TAncestorDraftCreate(-1, null, source, "rinnan") with { LAncestorDraftRelation = borrowed },
                TInterface.TAncestorDraftCreate(-2, null, null, "*rinnaną", string.Empty, tongue, "to run") with
                {
                    LAncestorDraftRelation = echo,
                },
                TInterface.TAncestorDraftCreate(-3, -2, null, "*rei-", string.Empty, null, "to flow"),
            ],
        };

        return engine.TEngineEntrySave(draft).LEntryId;
    }

    private static LMarkupEntry TMarkupAncestorExport(
        LEngine engine, TWorkspace workspace, long entryId, out string text)
    {
        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([entryId], path);
        text = File.ReadAllText(path);

        LMarkupEntry entry = Assert.Single(TInterface.TMarkupParse(text, out IReadOnlyList<LMarkupOmission> read));
        Assert.Empty(read);
        return entry;
    }

    private static IReadOnlyList<LAncestorDraft> TMarkupAncestorRead(LEngine engine, long entryId)
    {
        return Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entryId)).LEntryDraftAncestors;
    }

    private static IReadOnlyList<string> TAncestorShapeRead(IReadOnlyList<LAncestorDraft> map)
    {
        List<LAncestorDraft> boxes = [.. map];
        return boxes
            .Select(box => string.Join(
                "|",
                boxes.FindIndex(held => held.LAncestorDraftId == box.LAncestorDraftChild),
                box.LAncestorDraftTarget,
                box.LAncestorDraftText,
                box.LAncestorDraftLanguage,
                box.LAncestorDraftTongue,
                box.LAncestorDraftGloss,
                box.LAncestorDraftRelation))
            .ToList();
    }
}
