using System.Collections.Generic;
using System.IO;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupEtymology
{
    [Fact]
    public void MarkupExport_NarratedEtymology_WritesNamesNotIds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        TEtymologyProseSave(engine, entryId, "From rinnan.", 5, 6, source);

        LMarkupEntry entry = TEtymologyExport(engine, workspace, entryId, out string text);

        Assert.DoesNotContain("<etymon>", text, System.StringComparison.Ordinal);
        LMarkupEtymology etymology = Assert.IsType<LMarkupEtymology>(entry.LMarkupEntryEtymology);
        Assert.Equal("From rinnan.", etymology.LMarkupEtymologyText);
        LMarkupMention mention = Assert.Single(etymology.LMarkupEtymologyMention);
        Assert.Equal((5, 6, "rinnan", "English"), (
            mention.LMarkupMentionOffset,
            mention.LMarkupMentionLength,
            mention.LMarkupMentionHeadword,
            mention.LMarkupMentionLanguage));
        Assert.Empty(mention.LMarkupMentionSense);
    }

    [Fact]
    public void MarkupExport_LinkedEtymology_WritesOneEtymonPerSourceInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long first = TEtymologyEntryCreate(engine, "rinnan");
        long second = TEtymologyEntryCreate(engine, "iernan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        TEtymologySourceSave(engine, entryId, [first, second]);

        LMarkupEntry entry = TEtymologyExport(engine, workspace, entryId, out string text);

        Assert.DoesNotContain("<etymology>", text, System.StringComparison.Ordinal);
        Assert.Null(entry.LMarkupEntryEtymology);
        Assert.Equal(
            ["rinnan", "iernan"], entry.LMarkupEntryEtymon.Select(static etymon => etymon.LMarkupEtymonHeadword));
    }

    [Fact]
    public void MarkupImport_BothShapes_KeepsTheNarrativeAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");

        LEntry stored = TEtymologyImport(
            engine,
            workspace,
            """
              <etymon><headword>rinnan</headword><language>English</language></etymon>
              <etymology>
                <text>From rinnan.</text>
                <mention>
                  <offset>5</offset>
                  <length>6</length>
                  <headword>rinnan</headword>
                  <language>English</language>
                </mention>
              </etymology>
            """,
            out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Empty(omissions);
        LEtymologyDraft etymology = TEtymologyRead(engine, stored.LEntryId);
        Assert.Equal("From rinnan.", etymology.LEtymologyDraftText);
        Assert.Empty(etymology.LEtymologyDraftEtymons);
        Assert.Equal(source, Assert.Single(etymology.LEtymologyDraftMentions).LMentionDraftEntry);
    }

    [Fact]
    public void MarkupImport_EtymonNamingNoEntry_ReportsAnOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = TEtymologyImport(
            engine,
            workspace,
            "  <etymon><headword>rinnan</headword><language>English</language></etymon>",
            out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Equal("etymon \"rinnan\"", Assert.Single(omissions).LMarkupOmissionText);
        Assert.True(TEtymologyRead(engine, stored.LEntryId).LEtymologyDraftEmpty);
    }

    [Fact]
    public void MarkupImport_SpanOutOfRangeOrNamingNoEntry_ReportsAnOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = TEtymologyImport(
            engine,
            workspace,
            """
              <etymology>
                <text>From rinnan.</text>
                <mention><offset>5</offset><length>90</length><headword>rinnan</headword></mention>
                <mention><offset>0</offset><length>4</length></mention>
                <mention><offset>5</offset><length>6</length><sense>1</sense><headword>rinnan</headword></mention>
              </etymology>
            """,
            out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Equal(
            ["<sense>", "mention \"rinnan\"", "mention at 0", "mention at 5"],
            omissions.Select(static omission => omission.LMarkupOmissionText).Order(System.StringComparer.Ordinal));
        Assert.Empty(TEtymologyRead(engine, stored.LEntryId).LEtymologyDraftMentions);
    }

    [Fact]
    public void MarkupImport_MergeOverAStoredEtymology_LeavesTheStoredOneAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        TEtymologyProseSave(engine, entryId, "From rinnan.", 5, 6, source);

        string path = workspace.TWorkspaceMarkupSave("""
            <llyn>
              <entry>
                <headword>run</headword>
                <language>English</language>
                <etymon><headword>rinnan</headword><language>English</language></etymon>
              </entry>
            </llyn>
            """);
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);
        engine.TEngineMarkupImport(
            cargo, [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, entryId)]);

        LEtymologyDraft etymology = TEtymologyRead(engine, entryId);
        Assert.Equal("From rinnan.", etymology.LEtymologyDraftText);
        Assert.Empty(etymology.LEtymologyDraftEtymons);
    }

    [Fact]
    public void MarkupExport_NarratedEtymology_ImportsBackTheSameWay()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");
        TEtymologyProseSave(engine, entryId, "From rinnan.", 5, 6, source);

        TEtymologyExport(engine, workspace, entryId, out string text);
        string path = workspace.TWorkspaceMarkupSave(text.Replace(
            "<headword>run</headword>", "<headword>runs</headword>", System.StringComparison.Ordinal));
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(
            cargo, [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Empty(outcome.LMarkupOutcomeOmission);
        LEtymologyDraft etymology = TEtymologyRead(engine, Assert.Single(outcome.LMarkupOutcomeEntry).LEntryId);
        Assert.Equal("From rinnan.", etymology.LEtymologyDraftText);
        LMentionDraft mention = Assert.Single(etymology.LEtymologyDraftMentions);
        Assert.Equal((5, 6, source), (
            mention.LMentionDraftOffset, mention.LMentionDraftLength, mention.LMentionDraftEntry));
    }

    private static LMarkupEntry TEtymologyExport(
        LEngine engine, TWorkspace workspace, long entryId, out string text)
    {
        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([entryId], path);
        text = File.ReadAllText(path);

        LMarkupEntry entry = Assert.Single(TInterface.TMarkupParse(text, out IReadOnlyList<LMarkupOmission> read));
        Assert.Empty(read);
        return entry;
    }

    private static LEntry TEtymologyImport(
        LEngine engine, TWorkspace workspace, string body, out IReadOnlyList<LMarkupOmission> omissions)
    {
        string path = workspace.TWorkspaceMarkupSave(
            "<llyn>\n  <entry>\n    <headword>run</headword>\n    <language>English</language>\n"
            + body
            + "\n  </entry>\n</llyn>");
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(
            cargo, [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        omissions = outcome.LMarkupOutcomeOmission;
        return Assert.Single(outcome.LMarkupOutcomeEntry);
    }

    private static void TEtymologyProseSave(
        LEngine engine, long entryId, string text, int offset, int length, long source)
    {
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, text));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, offset, length, source));
        engine.TEngineDraftCommit(held.LDraftId);
    }

    private static void TEtymologySourceSave(LEngine engine, long entryId, IReadOnlyList<long> sources)
    {
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        foreach (long source in sources)
        {
            engine.TEngineRequestApply(
                TInterface.TEtymonAdditionCreate(held.LDraftId, source, sources.Count));
        }

        engine.TEngineDraftCommit(held.LDraftId);
    }

    private static LEtymologyDraft TEtymologyRead(LEngine engine, long entryId)
    {
        return Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entryId)).LEntryDraftEtymology;
    }

    private static long TEtymologyEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }
}
