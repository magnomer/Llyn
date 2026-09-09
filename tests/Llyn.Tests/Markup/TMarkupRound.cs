using System.Text;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupRound
{
    private const string TMarkupSample =
        """
        <llyn>
          <catalog>
            <author id="murray">Murray, James</author>
            <author id="bradley">Bradley, Henry</author>

            <source id="oed">
              <title>Oxford English Dictionary</title>
              <author ref="murray"/>
              <author ref="bradley"/>
              <year>1928</year>
              <kind>book</kind>
              <url>https://www.oed.com/</url>
            </source>

            <example id="ex-brush" lang="English" src="oed">
              <text>he kindled the dry brush with a single match</text>
              <trans>그는 성냥 하나로 마른 덤불에 불을 붙였다</trans>
            </example>

            <example id="ex-hope" lang="English">
              <text>the speech kindled a hope none of them had voiced</text>
            </example>

            <situation id="hearth">
              <title>Around a hearth</title>
              <description>The wording a cold evening invites.</description>
              <kind>speaking</kind>
            </situation>

            <register id="formal" lang="English">
              <name>formal</name>
            </register>

            <image id="fire">media/fire.jpg</image>

            <video id="clip">
              <location>media/kindling.mp4</location>
              <span>00:12-00:19</span>
            </video>
          </catalog>

          <entry id="kindle">
            <headword>kindle</headword>
            <lang>English</lang>
            <note>Chiefly literary in its figurative senses.</note>

            <form role="past" local="과거형">kindled</form>
            <form role="participle">kindling</form>
            <pos id="verb"/>
            <pos>서술어</pos>

            <inflection local="3인칭 단수" pos="verb">
              <text>kindles</text>
              <feature id="person" value="third"/>
              <feature id="number" value="singular"/>
            </inflection>

            <inflection local="과거">
              <text>kindled</text>
              <feature id="tense" value="past"/>
            </inflection>

            <pronunciation level="standard">
              <ipa>/ˈkɪnd(ə)l/</ipa>
              <syllable onset="k" nucleus="ɪ" coda="n" orthography="kin" local="킨" tone="1"/>
              <syllable onset="d" nucleus="ə" coda="l" tone-local="낮음" tone-points="13"/>
              <representation system="revised" role="transcription">kindeul</representation>
              <representation system="yale" role="reading" tone="평성">khintul</representation>
              <audio source="recorded by the author">media/kindle.mp3</audio>
            </pronunciation>

            <sense id="s-alight">
              <title>set alight</title>
              <gloss>ignite</gloss>
              <meaning lang="English">to set something burning</meaning>
              <labels>literal</labels>
              <use ref="ex-brush"/>
              <use ref="ex-hope" par="with" dep="Instrument"/>
              <use par="for" dep="Agent"/>
              <situation ref="hearth"/>
              <register ref="formal"/>
              <image ref="fire"/>
              <video ref="clip"/>
              <tag>fire</tag>
              <tag>warmth</tag>
              <translation entry="점화하다"/>

              <sense id="s-figurative">
                <meaning>to rouse a feeling</meaning>
                <use ref="ex-hope"/>
              </sense>
            </sense>

            <collocation id="c-interest">
              <title>kindle interest</title>
              <expression>kindle interest</expression>
              <meaning>to make someone begin to care</meaning>
              <use ref="ex-brush"/>
              <tag>figurative</tag>
            </collocation>
          </entry>

          <entry id="점화하다">
            <headword>점화하다</headword>
            <lang>Korean</lang>
            <sense><meaning lang="Korean">불을 붙이다</meaning></sense>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupExport_EveryFieldOfAnEntry_ComesBackTheSame()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using LEngine one = first.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported = one.TEngineMarkupImport(first.TWorkspaceMarkupSave(TMarkupSample));
        string path = Path.Combine(first.TWorkspaceFolder, "export.llx");
        one.TEngineMarkupExport(imported.Select(entry => entry.LEntryId).ToList(), path);

        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine other = second.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> reimported = other.TEngineMarkupImport(
            second.TWorkspaceMarkupSave(File.ReadAllText(path)));

        Assert.Equal(imported.Count, reimported.Count);
        for (int place = 0; place < imported.Count; place++)
        {
            Assert.Equal(
                TMarkupRoundFormat(one, imported[place].LEntryId),
                TMarkupRoundFormat(other, reimported[place].LEntryId));
        }
    }

    [Fact]
    public void MarkupExport_TheSameWorkspaceTwice_WritesTheSameBytes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<string> ids = engine
            .TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupSample))
            .Select(entry => entry.LEntryId)
            .ToList();

        string one = Path.Combine(workspace.TWorkspaceFolder, "one.llx");
        string other = Path.Combine(workspace.TWorkspaceFolder, "other.llx");
        engine.TEngineMarkupExport(ids, one);
        engine.TEngineMarkupExport(ids, other);

        Assert.Equal(File.ReadAllBytes(one), File.ReadAllBytes(other));
    }

    [Fact]
    public void MarkupExport_ExampleQuotedTwice_WritesOneCatalogRow()
    {
        string written = TMarkupRoundExport(TMarkupSample, out _);

        Assert.Equal(2, TMarkupRoundFind(written, "<example id="));
        Assert.Equal(5, TMarkupRoundFind(written, "<use"));
        Assert.Equal(1, TMarkupRoundFind(written, "<source id="));
    }

    [Fact]
    public void MarkupExport_ExampleQuotedTwice_ImportsAsOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string written = TMarkupRoundExport(TMarkupSample, out _);
        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(written));

        Assert.Equal(2, engine.TEngineExampleRead().Count);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM video;"));
    }

    [Fact]
    public void MarkupExport_SubSenseTree_SurvivesASecondTrip()
    {
        string once = TMarkupRoundExport(TMarkupSample, out _);
        string twice = TMarkupRoundExport(once, out _);

        Assert.Equal(once, twice);
        Assert.Contains("to rouse a feeling", twice, StringComparison.Ordinal);
    }

    private const string TMarkupQuoted =
        """"
        <llyn>
          <entry>
            <headword>kindle</headword>
            <form role="past" local="he said ""it's mine""">kindled</form>
            <sense>
              <meaning>to set something burning</meaning>
              <use par="he said ""it's mine""" dep="Agent"/>
            </sense>
          </entry>
        </llyn>
        """";

    [Fact]
    public void MarkupExport_ValueHoldingBothQuotes_ComesBackWhole()
    {
        const string carried = "he said \"it's mine\"";
        string written = TMarkupRoundExport(TMarkupQuoted, out _);

        Assert.Contains("he said \"\"it's mine\"\"", written, StringComparison.Ordinal);

        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> reimported =
            engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(written));
        LEntryDraft? draft = engine.TEngineEntryLoad(reimported[0].LEntryId);
        Assert.NotNull(draft);

        Assert.Equal(carried, draft.LEntryDraftForms[0].LFormLocal);
        Assert.Equal(
            carried,
            draft.LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftParticle.TStateValueShow());
    }

    [Fact]
    public void MarkupImport_UncitedSourceAndUncreditedAuthor_AreCreated()
    {
        const string written =
            """
            <llyn>
              <catalog>
                <author id="gaskell">Gaskell, Ruth</author>
                <source id="dormant"><title>A Work Nothing Cites</title></source>
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
            </llyn>
            """;

        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(written));

        Assert.Equal(
            "A Work Nothing Cites",
            Assert.Single(engine.TEngineReferenceRead()).LReferenceTitle.TStateValueShow());
        Assert.Equal("Gaskell, Ruth", Assert.Single(engine.TEngineAuthorRead()).LAuthorName);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source_author;"));
    }

    [Fact]
    public void MarkupExport_SourceNothingQuotes_StaysOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported = engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(
            """
            <llyn>
              <catalog>
                <author id="gaskell">Gaskell, Ruth</author>
                <source id="dormant">
                  <title>A Work Nothing Cites</title>
                  <author ref="gaskell"/>
                </source>
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
              <entry><headword>smoulder</headword><lang>English</lang></entry>
            </llyn>
            """));

        string path = Path.Combine(workspace.TWorkspaceFolder, "one.llx");
        engine.TEngineMarkupExport([imported[1].LEntryId], path);
        string written = File.ReadAllText(path);

        Assert.Contains("<headword>smoulder</headword>", written, StringComparison.Ordinal);
        Assert.DoesNotContain("<source id=", written, StringComparison.Ordinal);
        Assert.DoesNotContain("<author id=", written, StringComparison.Ordinal);
    }

    [Fact]
    public void MarkupExport_TwoAuthorsOfOneName_StayTwoRows()
    {
        string written = TMarkupRoundExport(
            """
            <llyn>
              <catalog>
                <author id="one">Murray, James</author>
                <author id="two">Murray, James</author>
                <source id="oed">
                  <title>Oxford English Dictionary</title>
                  <author ref="one"/>
                  <author ref="two"/>
                </source>
                <example id="ex-brush" lang="English" src="oed">
                  <text>he kindled the dry brush</text>
                </example>
              </catalog>
              <entry>
                <headword>kindle</headword>
                <lang>English</lang>
                <sense><use ref="ex-brush"/></sense>
              </entry>
            </llyn>
            """,
            out _);

        Assert.Equal(2, TMarkupRoundFind(written, "<author id="));

        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(written));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM author;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source_author;"));
    }

    [Fact]
    public void MarkupExport_GlossPresentAndAbsent_KeepsBothAndNoThirdCase()
    {
        string written = TMarkupRoundExport(
            """
            <llyn>
              <entry>
                <headword>kindle</headword>
                <lang>English</lang>
                <sense><gloss>ignite</gloss><meaning>to set alight</meaning></sense>
                <sense><meaning>to rouse a feeling</meaning></sense>
              </entry>
            </llyn>
            """,
            out _);

        Assert.Equal(1, TMarkupRoundFind(written, "<gloss>"));
        Assert.Contains("<gloss>ignite</gloss>", written, StringComparison.Ordinal);
        Assert.DoesNotContain("<gloss></gloss>", written, StringComparison.Ordinal);

        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry stored = Assert.Single(
            engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(written)));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal("ignite", loaded.LEntryDraftMeanings[0].LCardDraftGloss);
        Assert.Null(loaded.LEntryDraftMeanings[1].LCardDraftGloss);
    }

    [Fact]
    public void MarkupExport_FrameWithNoExample_ComesBackAsAFrame()
    {
        string written = TMarkupRoundExport(
            """
            <llyn>
              <entry>
                <headword>kindle</headword>
                <lang>English</lang>
                <sense><use par="for" dep="Agent"/></sense>
              </entry>
            </llyn>
            """,
            out _);

        Assert.Contains("<use par=\"for\" dep=\"Agent\"/>", written, StringComparison.Ordinal);
        Assert.DoesNotContain("<example id=", written, StringComparison.Ordinal);
    }

    [Fact]
    public void MarkupExport_TranslationBetweenEntries_KeepsThePointer()
    {
        string written = TMarkupRoundExport(TMarkupSample, out _);

        Assert.Contains("<translation entry=", written, StringComparison.Ordinal);

        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LEntry> imported = engine.TEngineMarkupImport(
            workspace.TWorkspaceMarkupSave(written));

        LEntryDraft? loaded = engine.TEngineEntryLoad(
            imported.Single(entry => entry.LEntryHeadword == "kindle").LEntryId);
        Assert.NotNull(loaded);

        string target = Assert.Single(loaded.LEntryDraftMeanings[0].LCardDraftTranslation);
        Assert.Equal("점화하다", engine.TEngineEntryRead(target)?.LEntryHeadword);
    }

    private static string TMarkupRoundExport(string text, out IReadOnlyList<LEntry> imported)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        imported = engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(text));
        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport(imported.Select(entry => entry.LEntryId).ToList(), path);

        return File.ReadAllText(path);
    }

    private static int TMarkupRoundFind(string text, string named)
    {
        int found = 0;
        int position = text.IndexOf(named, StringComparison.Ordinal);
        while (position >= 0)
        {
            found++;
            position = text.IndexOf(named, position + named.Length, StringComparison.Ordinal);
        }

        return found;
    }

    [Fact]
    public void MarkupExport_CustomSpeech_StaysCustom()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using LEngine one = first.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported = one.TEngineMarkupImport(first.TWorkspaceMarkupSave(TMarkupSample));
        string path = Path.Combine(first.TWorkspaceFolder, "export.llx");
        one.TEngineMarkupExport(imported.Select(entry => entry.LEntryId).ToList(), path);

        string written = File.ReadAllText(path);
        Assert.Contains("<pos id=\"verb\"/>", written);
        Assert.Contains("<pos>서술어</pos>", written);

        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine other = second.TWorkspaceEngineStart();

        LEntry kindle = other
            .TEngineMarkupImport(second.TWorkspaceMarkupSave(written))
            .First(entry => entry.LEntryHeadword == "kindle");

        IReadOnlyList<LSpeech> speeches =
            TInterface.TEntryArchiveCreate(second.TWorkspaceDatabase).TEntrySpeechRead(kindle.LEntryId);

        Assert.Equal(2, speeches.Count);
        Assert.Equal("verb", speeches[0].LSpeechValueId);
        Assert.Null(speeches[0].LSpeechCustom);
        Assert.Null(speeches[1].LSpeechValueId);
        Assert.Equal("서술어", speeches[1].LSpeechCustom);
    }

    private static string TMarkupRoundFormat(LEngine engine, string entryId)
    {
        LEntryDraft? draft = engine.TEngineEntryLoad(entryId);
        Assert.NotNull(draft);

        StringBuilder text = new();
        text.Append("headword=").Append(draft.LEntryDraftHeadword).Append('\n');
        text.Append("lang=").Append(draft.LEntryDraftLanguage).Append('\n');
        text.Append("note=").Append(draft.LEntryDraftNote).Append('\n');
        text.Append("ipa=").Append(draft.LEntryDraftIpa).Append('\n');
        text.Append("audio=").Append(Path.GetFileName(draft.LEntryDraftAudio)).Append('\n');
        text.Append("source=")
            .Append(draft.LEntryDraftPronunciation?.LPronunciationDraftSource ?? string.Empty)
            .Append('\n');
        text.Append("level=")
            .Append(draft.LEntryDraftPronunciation?.LPronunciationDraftLevel ?? string.Empty)
            .Append('\n');
        TMarkupRoundFormat(text, draft);

        TMarkupRoundFormat(engine, text, "sense", draft.LEntryDraftMeanings);
        TMarkupRoundFormat(engine, text, "collocation", draft.LEntryDraftCollocations);
        return text.ToString();
    }

    private static void TMarkupRoundFormat(StringBuilder text, LEntryDraft draft)
    {
        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            text.Append("pos id=").Append(speech.LSpeechDraftValue ?? "-")
                .Append(" custom=").Append(speech.LSpeechDraftCustom ?? "-").Append('\n');
        }

        foreach (LForm form in draft.LEntryDraftForms)
        {
            text.Append("form=").Append(form.LFormText)
                .Append(" role=").Append(form.LFormRole)
                .Append(" local=").Append(form.LFormLocal ?? "-").Append('\n');
        }

        foreach (LInflection inflection in draft.LEntryDraftInflections)
        {
            text.Append("inflection=").Append(inflection.LInflectionText)
                .Append(" local=").Append(inflection.LInflectionLocal ?? "-")
                .Append(" pos=").Append(inflection.LInflectionSpeechId ?? "-").Append('\n');

            foreach (LFeature feature in inflection.LInflectionFeatures)
            {
                text.Append(" feature=").Append(feature.LFeatureId)
                    .Append(' ').Append(feature.LFeatureValueId).Append('\n');
            }
        }

        if (draft.LEntryDraftPronunciation is not LPronunciationDraft spoken)
        {
            return;
        }

        foreach (LSyllable syllable in spoken.LPronunciationDraftSyllables)
        {
            text.Append("syllable")
                .Append(" onset=").Append(syllable.LSyllableOnset ?? "-")
                .Append(" medial=").Append(syllable.LSyllableMedial ?? "-")
                .Append(" nucleus=").Append(syllable.LSyllableNucleus)
                .Append(" coda=").Append(syllable.LSyllableCoda ?? "-")
                .Append(" orthography=").Append(syllable.LSyllableOrthography ?? "-")
                .Append(" local=").Append(syllable.LSyllableLocal ?? "-")
                .Append(" tone=").Append(syllable.LSyllableToneNumber?.ToString() ?? "-")
                .Append(" tone-local=").Append(syllable.LSyllableToneLocal ?? "-")
                .Append(" tone-points=").Append(syllable.LSyllableTonePoints ?? "-").Append('\n');
        }

        foreach (LRepresentation representation in spoken.LPronunciationDraftRepresentations)
        {
            text.Append("representation=").Append(representation.LRepresentationText)
                .Append(" system=").Append(representation.LRepresentationSystem)
                .Append(" role=").Append(representation.LRepresentationRole)
                .Append(" tone=").Append(representation.LRepresentationLocalTone ?? "-").Append('\n');
        }
    }

    private static void TMarkupRoundFormat(
        LEngine engine, StringBuilder text, string block, IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            text.Append(block).Append(' ').Append(card.LCardDraftPosition).Append('\n');
            text.Append(" title=").Append(TMarkupRoundShow(card.LCardDraftTitle)).Append('\n');
            text.Append(" gloss=").Append(card.LCardDraftGloss ?? "-").Append('\n');
            text.Append(" meaning=").Append(TMarkupRoundShow(card.LCardDraftMeaning)).Append('\n');
            text.Append(" lang=").Append(card.LCardDraftLanguage ?? "-").Append('\n');
            text.Append(" labels=").Append(card.LCardDraftLabels).Append('\n');
            text.Append(" expression=").Append(TMarkupRoundShow(card.LCardDraftExpression)).Append('\n');

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                text.Append("  use ")
                    .Append(TMarkupRoundShow(sentence.LSentenceDraftParticle))
                    .Append(' ')
                    .Append(TMarkupRoundShow(sentence.LSentenceDraftDependence));

                if (sentence.LSentenceDraftExample is LExampleDraft quoted)
                {
                    text.Append(" | ")
                        .Append(TMarkupRoundShow(quoted.LExampleDraftText))
                        .Append(" | ")
                        .Append(TMarkupRoundShow(quoted.LExampleDraftTranslation))
                        .Append(" | ")
                        .Append(quoted.LExampleDraftLanguage)
                        .Append(" | ")
                        .Append(TMarkupRoundRead(engine, quoted.LExampleDraftReference));
                }

                text.Append('\n');
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                text.Append("  situation ")
                    .Append(TMarkupRoundShow(situation.LSituationDraftTitle))
                    .Append(" | ")
                    .Append(TMarkupRoundShow(situation.LSituationDraftDescription))
                    .Append(" | ")
                    .Append(TMarkupRoundShow(situation.LSituationDraftKind))
                    .Append('\n');
            }

            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                text.Append("  register ")
                    .Append(TMarkupRoundShow(register.LRegisterDraftName))
                    .Append(" | ")
                    .Append(register.LRegisterDraftLanguage)
                    .Append('\n');
            }

            foreach (LImageDraft image in card.LCardDraftImage)
            {
                text.Append("  image ").Append(TMarkupRoundShow(image.LImageDraftLocation)).Append('\n');
            }

            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                text.Append("  video ")
                    .Append(TMarkupRoundShow(video.LVideoDraftLocation))
                    .Append(" | ")
                    .Append(TMarkupRoundShow(video.LVideoDraftSpan))
                    .Append('\n');
            }

            foreach (string tag in card.LCardDraftTag)
            {
                text.Append("  tag ").Append(tag).Append('\n');
            }

            foreach (string translation in card.LCardDraftTranslation)
            {
                text.Append("  translation ")
                    .Append(engine.TEngineEntryRead(translation)?.LEntryHeadword ?? "-")
                    .Append('\n');
            }

            TMarkupRoundFormat(engine, text, block + "-child", card.LCardDraftChild);
        }
    }

    private static string TMarkupRoundRead(LEngine engine, LStateValue citation)
    {
        if (citation.LStateValueState != LState.LStateSpecified)
        {
            return citation.LStateValueState.ToString();
        }

        LReference? held = engine.TEngineReferenceRead(citation.TStateValueShow());
        return held is null ? "-" : held.LReferenceTitle.TStateValueShow();
    }

    private static string TMarkupRoundShow(LStateValue value)
    {
        return value.LStateValueState switch
        {
            LState.LStateSpecified => value.TStateValueShow(),
            LState.LStateUnknown => "?",
            _ => "-",
        };
    }
}
