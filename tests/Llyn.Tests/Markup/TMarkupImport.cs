using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupImport
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
    public void MarkupImport_TwoEntries_StoresBothWithAllStates()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported = engine.TEngineMarkupImport(TMarkupSample);

        Assert.Equal(["kindle", "brook"], imported.Select(entry => entry.LEntryHeadword));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));

        LEntryDraft kindle = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(imported[0].LEntryId));
        Assert.Equal("kindle", kindle.LEntryDraftHeadword);
        Assert.Equal("English", kindle.LEntryDraftLanguage);
        Assert.Equal("/ˈkɪnd(ə)l/", kindle.LEntryDraftPronunciation);
        Assert.Equal("Chiefly literary in its figurative senses.", kindle.LEntryDraftNote);
        Assert.Equal(2, kindle.LEntryDraftMeanings.Count);

        LCardDraft alight = kindle.LEntryDraftMeanings[0];
        Assert.Equal("set alight", alight.LCardDraftTitle.TStateValueShow());
        Assert.Equal("to set something burning; to start a flame", alight.LCardDraftMeaning.TStateValueShow());
        Assert.Equal(["literal"], alight.LCardDraftTag);
        Assert.Equal(["media/kindle-hearth.jpg"], alight.LCardDraftImage.Select(image => image.TStateValueShow()));

        LExampleDraft logs = Assert.Single(alight.LCardDraftExample);
        LSituationDraft hearth = Assert.Single(alight.LCardDraftSituation);
        Assert.Equal("she knelt to kindle the damp logs", logs.LExampleDraftText.TStateValueShow());
        Assert.Equal("around a hearth on a cold evening", hearth.LSituationDraftText.TStateValueShow());

        LReference oed = Assert.Single(engine.TEngineReferenceRead(), reference =>
            reference.LReferenceTitle.TStateValueShow() == "Oxford English Dictionary");
        Assert.Equal(oed.LReferenceId, logs.LExampleDraftReference.TStateValueShow());
        Assert.Equal("1928", oed.LReferenceYear.TStateValueShow());
        Assert.Equal(LState.LStateUnknown, oed.LReferenceProgram.LStateValueState);
        Assert.Equal(LState.LStateSpecified, oed.LReferenceAuthorState);
        Assert.Equal(
            ["Murray, James"],
            engine.TEngineAuthorRead(oed.LReferenceId, LOwner.LOwnerReference).Select(author => author.LAuthorName));

        LCardDraft rouse = kindle.LEntryDraftMeanings[1];
        Assert.Equal(["figurative"], rouse.LCardDraftTag);
        Assert.Equal(
            [LState.LStateUnspecified, LState.LStateUnknown],
            rouse.LCardDraftExample.Select(example => example.LExampleDraftReference.LStateValueState));

        LCardDraft interest = Assert.Single(kindle.LEntryDraftCollocations);
        Assert.Equal("kindle interest", interest.LCardDraftExpression.TStateValueShow());
        Assert.Equal(
            "the exhibition kindled fresh interest in the painter",
            Assert.Single(interest.LCardDraftExample).LExampleDraftText.TStateValueShow());

        LEntryDraft brook = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(imported[1].LEntryId));
        Assert.Equal("brook", brook.LEntryDraftHeadword);
        Assert.Equal(2, brook.LEntryDraftMeanings.Count);
        Assert.Equal(
            ["formal", "usually negative"],
            brook.LEntryDraftMeanings[1].LCardDraftTag);
        Assert.Equal(
            LState.LStateUnknown,
            Assert.Single(brook.LEntryDraftMeanings[1].LCardDraftSituation).LSituationDraftText.LStateValueState);

        LReference field = Assert.Single(engine.TEngineReferenceRead(), reference =>
            reference.LReferenceTitle.TStateValueShow() == "A Field Guide to Rivers");
        Assert.Equal(LState.LStateUnknown, field.LReferenceAuthorState);
        Assert.Equal(LState.LStateUnknown, field.LReferenceUrl.LStateValueState);
        Assert.Empty(engine.TEngineAuthorRead(field.LReferenceId, LOwner.LOwnerReference));
        Assert.Equal(
            field.LReferenceId,
            Assert.Single(brook.LEntryDraftMeanings[0].LCardDraftExample)
                .LExampleDraftReference.TStateValueShow());
    }

    [Fact]
    public void MarkupImport_SourceCitedTwice_StoresOneReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = Assert.Single(engine.TEngineMarkupImport(
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

        LReference oed = Assert.Single(engine.TEngineReferenceRead());
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source;"));

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        IEnumerable<string> cited = draft.LEntryDraftMeanings.Select(card =>
            Assert.Single(card.LCardDraftExample).LExampleDraftReference.TStateValueShow());

        Assert.Equal([oed.LReferenceId, oed.LReferenceId], cited);
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead($"SELECT COUNT(*) FROM example WHERE source_id = '{oed.LReferenceId}';"));
    }

    [Fact]
    public void MarkupImport_UncitedSource_HoldsItUnderItsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = Assert.Single(engine.TEngineMarkupImport(
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
            engine.TEngineReferenceRead(entry.LEntryId, LOwner.LOwnerEntry)
                .Select(reference => reference.LReferenceTitle.TStateValueShow()));

        engine.TEngineEntryDelete(entry.LEntryId);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry_source;"));
    }

    [Fact]
    public void MarkupImport_AuthorNamedTwice_StoresOneAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineMarkupImport(
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
    public void MarkupImport_UnknownCitation_StoresNoEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Throws<FormatException>(() => engine.TEngineMarkupImport(
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
