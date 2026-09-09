using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupLink
{
    private const string TMarkupPair =
        """
        <llyn>
          <entry id="kindle">
            <headword>kindle</headword>
            <lang>English</lang>
            <sense id="s-alight">
              <meaning>to set something burning</meaning>
              <translation entry="ignite"/>
              <relation type="synonym" label="ignite" labels="유의어">
                <target entry="ignite"/>
              </relation>
            </sense>
            <collocation>
              <expression>kindle interest</expression>
              <meaning>to make someone begin to care</meaning>
              <synonym sense="s-rouse"/>
            </collocation>
          </entry>

          <entry id="ignite">
            <headword>ignite</headword>
            <lang>English</lang>
            <sense id="s-ignite">
              <meaning>to set burning</meaning>
              <relation type="antonym">
                <target sense="s-alight"/>
              </relation>
            </sense>
            <sense id="s-rouse">
              <meaning>to rouse a feeling</meaning>
            </sense>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupImport_RelationNamingALaterEntry_StoresTheLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported =
            engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupPair));

        LEntry kindle = imported.Single(entry => entry.LEntryHeadword == "kindle");
        LEntry ignite = imported.Single(entry => entry.LEntryHeadword == "ignite");

        LRelation relation = Assert.Single(
            engine.TEngineRelationRead(TMarkupLinkRead(engine, kindle.LEntryId, 0)));

        Assert.Equal("synonym", relation.LRelationType);
        Assert.Equal("ignite", relation.LRelationLabel);
        Assert.Equal("유의어", relation.LRelationLabels);
        Assert.Equal(ignite.LEntryId, relation.LRelationTargetEntry);
        Assert.Null(relation.LRelationTargetMeaning);
    }

    [Fact]
    public void MarkupImport_RelationNamingASenseOfAnotherEntry_StoresTheLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported =
            engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupPair));

        LEntry kindle = imported.Single(entry => entry.LEntryHeadword == "kindle");
        LEntry ignite = imported.Single(entry => entry.LEntryHeadword == "ignite");

        LRelation relation = Assert.Single(
            engine.TEngineRelationRead(TMarkupLinkRead(engine, ignite.LEntryId, 0)));

        Assert.Equal("antonym", relation.LRelationType);
        Assert.Null(relation.LRelationTargetEntry);
        Assert.Equal(
            TMarkupLinkRead(engine, kindle.LEntryId, 0), relation.LRelationTargetMeaning);
    }

    [Fact]
    public void MarkupImport_RelationWithNoTarget_Refuses()
    {
        TMarkupLinkCheck(
            """
            <llyn>
              <entry>
                <headword>kindle</headword>
                <lang>English</lang>
                <sense><meaning>to set alight</meaning><relation type="synonym"></relation></sense>
              </entry>
            </llyn>
            """,
            "Relation 1 of card 1 names no target.");
    }

    [Fact]
    public void MarkupImport_RelationWithNoType_Refuses()
    {
        TMarkupLinkCheck(
            """
            <llyn>
              <entry id="kindle">
                <headword>kindle</headword>
                <lang>English</lang>
                <sense><meaning>to set alight</meaning>
                  <relation><target entry="kindle"/></relation>
                </sense>
              </entry>
            </llyn>
            """,
            "Relation 1 of card 1 names no type.");
    }

    [Fact]
    public void MarkupImport_TargetNamingBothKinds_Refuses()
    {
        TMarkupLinkCheck(
            """
            <llyn>
              <entry id="kindle">
                <headword>kindle</headword>
                <lang>English</lang>
                <sense id="s-alight"><meaning>to set alight</meaning>
                  <relation type="synonym"><target entry="kindle" sense="s-alight"/></relation>
                </sense>
              </entry>
            </llyn>
            """,
            "Relation 1 of card 1 names both an entry and a sense.");
    }

    [Fact]
    public void MarkupImport_SynonymNamingNeitherKind_Refuses()
    {
        TMarkupLinkCheck(
            """
            <llyn>
              <entry>
                <headword>kindle</headword>
                <lang>English</lang>
                <collocation><expression>kindle interest</expression><synonym/></collocation>
              </entry>
            </llyn>
            """,
            "Synonym 1 of card 1 names neither an entry nor a sense.");
    }

    [Fact]
    public void MarkupImport_TranslationAndCollocationSynonym_ComeBackOnASecondTrip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported =
            engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupPair));

        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        Assert.Empty(engine.TEngineMarkupExport(
            imported.Select(entry => entry.LEntryId).ToList(), path));

        string written = File.ReadAllText(path);
        Assert.Contains("<translation entry=", written, StringComparison.Ordinal);
        Assert.Contains("<synonym sense=", written, StringComparison.Ordinal);
        Assert.Contains("<relation type=\"synonym\"", written, StringComparison.Ordinal);

        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine other = second.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> reimported =
            other.TEngineMarkupImport(second.TWorkspaceMarkupSave(written));

        LEntry kindle = reimported.Single(entry => entry.LEntryHeadword == "kindle");
        LEntry ignite = reimported.Single(entry => entry.LEntryHeadword == "ignite");

        LEntryDraft? loaded = other.TEngineEntryLoad(kindle.LEntryId);
        Assert.NotNull(loaded);

        Assert.Equal(
            ignite.LEntryId,
            Assert.Single(loaded.LEntryDraftMeanings[0].LCardDraftTranslation));

        LSynonym synonym = Assert.Single(
            other.TEngineSynonymRead(loaded.LEntryDraftCollocations[0].LCardDraftId));
        Assert.Null(synonym.LSynonymTargetEntry);
        Assert.Equal(
            TMarkupLinkRead(other, ignite.LEntryId, 1), synonym.LSynonymTargetMeaning);
    }

    [Fact]
    public void MarkupExport_RelationOutsideTheSet_IsReportedAndNotWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported =
            engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupPair));

        LEntry kindle = imported.Single(entry => entry.LEntryHeadword == "kindle");
        string path = Path.Combine(workspace.TWorkspaceFolder, "one.llx");

        IReadOnlyList<LMarkupLoss> lost = engine.TEngineMarkupExport([kindle.LEntryId], path);
        string written = File.ReadAllText(path);

        Assert.DoesNotContain("<relation", written, StringComparison.Ordinal);
        Assert.DoesNotContain("<translation", written, StringComparison.Ordinal);
        Assert.DoesNotContain("<synonym", written, StringComparison.Ordinal);

        Assert.Equal(3, lost.Count);
        Assert.All(lost, loss => Assert.Equal("kindle", loss.LMarkupLossEntry));
        Assert.Equal(
            ["translation", "relation", "synonym"],
            lost.Select(loss => loss.LMarkupLossKind));
        Assert.Equal("ignite", lost[0].LMarkupLossTarget);
        Assert.Equal("ignite", lost[1].LMarkupLossTarget);
    }

    [Fact]
    public void MarkupExport_TwoEntriesPointingAtEachOther_LeaveNoDanglingRow()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using LEngine one = first.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported =
            one.TEngineMarkupImport(first.TWorkspaceMarkupSave(TMarkupPair));

        string path = Path.Combine(first.TWorkspaceFolder, "export.llx");
        Assert.Empty(one.TEngineMarkupExport(
            imported.Select(entry => entry.LEntryId).ToList(), path));

        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine other = second.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> reimported =
            other.TEngineMarkupImport(second.TWorkspaceMarkupSave(File.ReadAllText(path)));

        Assert.Equal(2, reimported.Count);
        Assert.Equal(2, second.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(1, second.TWorkspaceCountRead("SELECT COUNT(*) FROM relation_entry;"));
        Assert.Equal(1, second.TWorkspaceCountRead("SELECT COUNT(*) FROM relation_sense;"));
        Assert.Equal(1, second.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_synonym;"));
        Assert.Empty(second.TWorkspaceRowRead("PRAGMA foreign_key_check;"));
    }

    private static void TMarkupLinkCheck(string text, string named)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        FormatException refused = Assert.Throws<FormatException>(
            () => engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(text)));

        Assert.Equal(named, refused.Message);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    private static string TMarkupLinkRead(LEngine engine, string entryId, int place)
    {
        LEntryDraft? draft = engine.TEngineEntryLoad(entryId);
        Assert.NotNull(draft);
        return draft.LEntryDraftMeanings[place].LCardDraftId;
    }
}
