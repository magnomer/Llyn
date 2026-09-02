using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TMarkup
{
    private const string TMarkupSample =
        """
        <entry>
          <headword>kindle</headword>
          <lang>English</lang>
          <ipa>/ˈkɪnd(ə)l/</ipa>
          <pos>verb</pos>
          <note>Chiefly literary in its figurative senses.</note>

          <sense>
            <title>set alight</title>
            <meaning>to set something burning; to start a flame</meaning>
            <tag>literal</tag>
            <example src="oed">she knelt to kindle the damp logs</example>
            <situation src="oed">around a hearth on a cold evening</situation>
            <image>media/kindle-hearth.jpg</image>
          </sense>

          <sense>
            <title>rouse a feeling</title>
            <meaning>to stir up an emotion or interest</meaning>
            <tag>figurative</tag>
            <tag></tag>
            <example>the teacher kindled a love of poetry in her class</example>
            <example src="">a remark that kindled old resentments</example>
          </sense>

          <collocation>
            <expression>kindle interest</expression>
            <meaning>to cause interest to begin</meaning>
            <example>the exhibition kindled fresh interest in the painter</example>
          </collocation>

          <source id="oed">
            <title>Oxford English Dictionary</title>
            <author>Murray, James</author>
            <year>1928</year>
            <url>https://www.oed.com/</url>
            <program></program>
            <channel></channel>
          </source>
        </entry>

        <entry>
          <headword>brook</headword>
          <lang>English</lang>
          <ipa>/brʊk/</ipa>

          <sense>
            <title>a small stream</title>
            <meaning>a small natural watercourse</meaning>
            <tag>nature</tag>
            <example src="field">the path followed a shallow brook down the valley</example>
          </sense>

          <sense>
            <title>to tolerate</title>
            <meaning>to bear or put up with, usually in the negative</meaning>
            <tag>formal</tag>
            <tag>usually negative</tag>
            <example>she would brook no argument on the matter</example>
            <situation></situation>
          </sense>

          <source id="field">
            <title>A Field Guide to Rivers</title>
            <author></author>
            <year>2011</year>
            <url></url>
          </source>
        </entry>
        """;

    [Fact]
    public void AFileOfTwoEntriesImportsAsTwoEntriesThatReadBackWithAllThreeStates()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        IReadOnlyList<LEntry> imported = engine.LEngineMarkupImport(TMarkupSample);

        Assert.Equal(["kindle", "brook"], imported.Select(entry => entry.LEntryHeadword));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));

        LEntryDraft kindle = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(imported[0].LEntryId));
        Assert.Equal("kindle", kindle.LEntryDraftHeadword);
        Assert.Equal("English", kindle.LEntryDraftLanguage);
        Assert.Equal("/ˈkɪnd(ə)l/", kindle.LEntryDraftPronunciation);
        Assert.Equal("Chiefly literary in its figurative senses.", kindle.LEntryDraftNote);
        Assert.Equal(2, kindle.LEntryDraftSenses.Count);

        LCardDraft alight = kindle.LEntryDraftSenses[0];
        Assert.Equal("set alight", alight.LCardDraftTitle.LStateValueShow());
        Assert.Equal("to set something burning; to start a flame", alight.LCardDraftMeaning.LStateValueShow());
        Assert.Equal(["literal"], alight.LCardDraftTag);
        Assert.Equal(["media/kindle-hearth.jpg"], alight.LCardDraftImage.Select(image => image.LStateValueShow()));

        LExampleDraft logs = Assert.Single(alight.LCardDraftExample);
        LSituationDraft hearth = Assert.Single(alight.LCardDraftSituation);
        Assert.Equal("she knelt to kindle the damp logs", logs.LExampleDraftText.LStateValueShow());
        Assert.Equal("around a hearth on a cold evening", hearth.LSituationDraftText.LStateValueShow());

        LReference oed = Assert.Single(engine.LEngineReferenceRead(), reference =>
            reference.LReferenceTitle.LStateValueShow() == "Oxford English Dictionary");
        Assert.Equal(oed.LReferenceId, logs.LExampleDraftReference.LStateValueShow());
        Assert.Equal(oed.LReferenceId, hearth.LSituationDraftReference.LStateValueShow());
        Assert.Equal("1928", oed.LReferenceYear.LStateValueShow());
        Assert.Equal(LState.LStateUnknown, oed.LReferenceProgram.LStateValueState);
        Assert.Equal(LState.LStateSpecified, oed.LReferenceAuthorState);
        Assert.Equal(
            ["Murray, James"],
            engine.LEngineAuthorRead(oed.LReferenceId, LOwner.LOwnerReference).Select(author => author.LAuthorName));

        LCardDraft rouse = kindle.LEntryDraftSenses[1];
        Assert.Equal(["figurative"], rouse.LCardDraftTag);
        Assert.Equal(
            [LState.LStateUnspecified, LState.LStateUnknown],
            rouse.LCardDraftExample.Select(example => example.LExampleDraftReference.LStateValueState));

        LCardDraft interest = Assert.Single(kindle.LEntryDraftCollocations);
        Assert.Equal("kindle interest", interest.LCardDraftExpression.LStateValueShow());
        Assert.Equal(
            "the exhibition kindled fresh interest in the painter",
            Assert.Single(interest.LCardDraftExample).LExampleDraftText.LStateValueShow());

        LEntryDraft brook = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(imported[1].LEntryId));
        Assert.Equal("brook", brook.LEntryDraftHeadword);
        Assert.Equal(2, brook.LEntryDraftSenses.Count);
        Assert.Equal(
            ["formal", "usually negative"],
            brook.LEntryDraftSenses[1].LCardDraftTag);
        Assert.Equal(
            LState.LStateUnknown,
            Assert.Single(brook.LEntryDraftSenses[1].LCardDraftSituation).LSituationDraftText.LStateValueState);

        LReference field = Assert.Single(engine.LEngineReferenceRead(), reference =>
            reference.LReferenceTitle.LStateValueShow() == "A Field Guide to Rivers");
        Assert.Equal(LState.LStateUnknown, field.LReferenceAuthorState);
        Assert.Equal(LState.LStateUnknown, field.LReferenceUrl.LStateValueState);
        Assert.Empty(engine.LEngineAuthorRead(field.LReferenceId, LOwner.LOwnerReference));
        Assert.Equal(
            field.LReferenceId,
            Assert.Single(brook.LEntryDraftSenses[0].LCardDraftExample)
                .LExampleDraftReference.LStateValueShow());
    }

    [Fact]
    public void OneSourceCitedByTwoExamplesBecomesOneReferenceRowCitedTwice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = Assert.Single(engine.LEngineMarkupImport(
            """
            <entry>
              <headword>kindle</headword>
              <lang>English</lang>

              <sense>
                <meaning>to set something burning</meaning>
                <example src="oed">she knelt to kindle the damp logs</example>
              </sense>

              <sense>
                <meaning>to stir up an emotion</meaning>
                <example src="oed">the teacher kindled a love of poetry</example>
              </sense>

              <source id="oed">
                <title>Oxford English Dictionary</title>
              </source>
            </entry>
            """));

        LReference oed = Assert.Single(engine.LEngineReferenceRead());
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source;"));

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(entry.LEntryId));
        IEnumerable<string> cited = draft.LEntryDraftSenses.Select(card =>
            Assert.Single(card.LCardDraftExample).LExampleDraftReference.LStateValueShow());

        Assert.Equal([oed.LReferenceId, oed.LReferenceId], cited);
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead($"SELECT COUNT(*) FROM example WHERE source_id = '{oed.LReferenceId}';"));
    }

    [Fact]
    public void ASourceNoCitationNamesIsStillHeldByTheEntryThatDeclaresIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = Assert.Single(engine.LEngineMarkupImport(
            """
            <entry>
              <headword>kindle</headword>
              <lang>English</lang>

              <sense>
                <meaning>to set something burning</meaning>
                <example src="oed">she knelt to kindle the damp logs</example>
              </sense>

              <source id="oed"><title>Oxford English Dictionary</title></source>
              <source id="field"><title>A Field Guide to Rivers</title></source>
            </entry>
            """));

        Assert.Equal(
            ["Oxford English Dictionary", "A Field Guide to Rivers"],
            engine.LEngineReferenceRead(entry.LEntryId, LOwner.LOwnerEntry)
                .Select(reference => reference.LReferenceTitle.LStateValueShow()));

        engine.LEngineEntryDelete(entry.LEntryId);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry_source;"));
    }

    [Fact]
    public void OneAuthorNamedByTwoSourcesBecomesOneAuthorRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        engine.LEngineMarkupImport(
            """
            <entry>
              <headword>kindle</headword>
              <lang>English</lang>
              <sense><meaning>to set something burning</meaning></sense>
              <source id="one">
                <title>Oxford English Dictionary</title>
                <author>Murray, James</author>
              </source>
              <source id="two">
                <title>A Field Guide to Rivers</title>
                <author>Murray, James</author>
              </source>
            </entry>
            """);

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM author;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source_author;"));
    }

    [Fact]
    public void AnUnknownCitationInTheSecondEntryLeavesTheWorkspaceWithoutAnyEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        Assert.Throws<FormatException>(() => engine.LEngineMarkupImport(
            """
            <entry>
              <headword>kindle</headword>
              <lang>English</lang>

              <sense>
                <meaning>to set something burning</meaning>
              </sense>
            </entry>

            <entry>
              <headword>brook</headword>
              <lang>English</lang>

              <sense>
                <meaning>a small natural watercourse</meaning>
                <example src="field">the path followed a shallow brook</example>
              </sense>
            </entry>
            """));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source;"));
    }
}
