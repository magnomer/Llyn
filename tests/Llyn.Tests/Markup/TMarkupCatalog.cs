using Llyn.Core;
using Xunit;

using LMarkupCatalog = Llyn.Core.LMarkup.LMarkupCatalog;
using LMarkupDocument = Llyn.Core.LMarkup.LMarkupDocument;
using LMarkupRowKind = Llyn.Core.LMarkup.LMarkupRowKind;

namespace Llyn.Tests;

public sealed class TMarkupCatalog
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

            <situation id="hearth">
              <title>Around a hearth</title>
              <description>The wording a cold evening invites.</description>
              <kind>speaking</kind>
            </situation>

            <register id="formal" lang="English" builtin="yes">
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
            <pos>verb</pos>

            <sense id="s-alight">
              <title>set alight</title>
              <meaning>to set something burning; to start a flame</meaning>
              <tag>fire</tag>
              <image ref="fire"/>
              <use ref="ex-brush"/>
            </sense>

            <collocation id="c-interest">
              <expression>kindle interest</expression>
              <meaning>to make someone begin to care about something</meaning>
            </collocation>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupEntryRead_Catalog_ReturnsEveryRow()
    {
        LMarkupCatalog catalog = TInterface.TMarkupEntryRead(TMarkupSample).LMarkupDocumentCatalog;

        Assert.Equal("Murray, James", catalog.LMarkupCatalogAuthor["murray"].LAuthorName);
        Assert.Equal(
            "Oxford English Dictionary",
            catalog.LMarkupCatalogSource["oed"].LMarkupReferenceValue.LReferenceTitle.TStateValueShow());
        Assert.Equal(["murray", "bradley"], catalog.LMarkupCatalogSource["oed"].LMarkupReferenceAuthor);

        LExample brush = catalog.LMarkupCatalogExample["ex-brush"];
        Assert.Equal("English", brush.LExampleLanguage);
        Assert.Equal("he kindled the dry brush with a single match", brush.LExampleText.TStateValueShow());
        Assert.Equal("그는 성냥 하나로 마른 덤불에 불을 붙였다", brush.LExampleTranslation.TStateValueShow());
        Assert.Equal("oed", brush.LExampleSource.TStateValueShow());

        LSituation hearth = catalog.LMarkupCatalogSituation["hearth"];
        Assert.Equal("Around a hearth", hearth.LSituationTitle.TStateValueShow());
        Assert.Equal("The wording a cold evening invites.", hearth.LSituationDescription.TStateValueShow());
        Assert.Equal("speaking", hearth.LSituationKind.TStateValueShow());

        LRegister formal = catalog.LMarkupCatalogRegister["formal"];
        Assert.Equal("formal", formal.LRegisterName.TStateValueShow());
        Assert.Equal("English", formal.LRegisterLanguage);
        Assert.True(formal.LRegisterBuiltin);

        Assert.Equal("media/fire.jpg", catalog.LMarkupCatalogImage["fire"].LImageLocation.TStateValueShow());
        Assert.Equal("media/kindling.mp4", catalog.LMarkupCatalogVideo["clip"].LVideoLocation.TStateValueShow());
        Assert.Equal("00:12-00:19", catalog.LMarkupCatalogVideo["clip"].LVideoSpan.TStateValueShow());
    }

    [Fact]
    public void MarkupEntryRead_EveryKey_CarriesTheKindDeclared()
    {
        LMarkupCatalog catalog = TInterface.TMarkupEntryRead(TMarkupSample).LMarkupDocumentCatalog;

        Assert.Equal(LMarkupRowKind.LMarkupRowAuthor, catalog.LMarkupCatalogRow["murray"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowSource, catalog.LMarkupCatalogRow["oed"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowExample, catalog.LMarkupCatalogRow["ex-brush"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowSituation, catalog.LMarkupCatalogRow["hearth"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowRegister, catalog.LMarkupCatalogRow["formal"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowImage, catalog.LMarkupCatalogRow["fire"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowVideo, catalog.LMarkupCatalogRow["clip"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowEntry, catalog.LMarkupCatalogRow["kindle"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowSense, catalog.LMarkupCatalogRow["s-alight"]);
    }

    [Fact]
    public void MarkupEntryRead_EntryKey_KeepsItOnTheEntry()
    {
        LMarkupDocument document = TInterface.TMarkupEntryRead(TMarkupSample);

        Assert.Equal("kindle", Assert.Single(document.LMarkupDocumentEntry).LMarkupEntryKey);
    }

    [Fact]
    public void MarkupEntryRead_AuthorAfterSource_ResolvesIt()
    {
        LMarkupCatalog catalog = TInterface.TMarkupEntryRead(
            """
            <llyn>
              <catalog>
                <source id="dormant">
                  <title>A Work Nothing Cites</title>
                  <author ref="gaskell"/>
                </source>
                <author id="gaskell">Gaskell, Ruth</author>
              </catalog>
            </llyn>
            """).LMarkupDocumentCatalog;

        Assert.Equal(
            ["gaskell"],
            catalog.LMarkupCatalogSource["dormant"].LMarkupReferenceAuthor);
        Assert.Equal("Gaskell, Ruth", catalog.LMarkupCatalogAuthor["gaskell"].LAuthorName);
    }

    [Fact]
    public void MarkupEntryRead_NestedSenseKey_KeepsBothKeys()
    {
        LMarkupCatalog catalog = TInterface.TMarkupEntryRead(
            """
            <llyn>
              <entry>
                <headword>kindle</headword>
                <sense id="alight">
                  <sense id="figurative"><meaning>to rouse a feeling</meaning></sense>
                </sense>
              </entry>
            </llyn>
            """).LMarkupDocumentCatalog;

        Assert.Equal(LMarkupRowKind.LMarkupRowSense, catalog.LMarkupCatalogRow["alight"]);
        Assert.Equal(LMarkupRowKind.LMarkupRowSense, catalog.LMarkupCatalogRow["figurative"]);
    }

    [Fact]
    public void MarkupEntryRead_KeyDeclaredTwice_ThrowsNamingKey()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupEntryRead(
            """
            <llyn>
              <catalog>
                <source id="oed"><title>Oxford English Dictionary</title></source>
                <image id="oed">media/fire.jpg</image>
              </catalog>
            </llyn>
            """));

        Assert.Contains("oed", failure.Message);
    }

    [Fact]
    public void MarkupEntryRead_DanglingReference_ThrowsNamingKey()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupEntryRead(
            """
            <llyn>
              <catalog>
                <example id="ex1" lang="English" src="oed"><text>she kindled the lamp</text></example>
              </catalog>
            </llyn>
            """));

        Assert.Contains("oed", failure.Message);
    }

    [Fact]
    public void MarkupEntryRead_WrongKind_ThrowsNamingBothKinds()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupEntryRead(
            """
            <llyn>
              <catalog>
                <source id="oed"><title>Oxford English Dictionary</title></source>
              </catalog>
              <entry>
                <headword>kindle</headword>
                <sense><situation ref="oed"/></sense>
              </entry>
            </llyn>
            """));

        Assert.Contains("oed", failure.Message);
        Assert.Contains("source", failure.Message);
        Assert.Contains("situation", failure.Message);
    }

    [Fact]
    public void MarkupEntryRead_NoRootElement_Throws()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupEntryRead(
            "<entry><headword>kindle</headword></entry>"));

        Assert.Contains("llyn", failure.Message);
    }

    [Fact]
    public void MarkupEntryRead_EntryOutsideRoot_ReturnsNoEntry()
    {
        LMarkupDocument document = TInterface.TMarkupEntryRead(
            "<llyn></llyn><entry><headword>kindle</headword></entry>");

        Assert.Empty(document.LMarkupDocumentEntry);
    }

    [Fact]
    public void MarkupRead_CompleteSample_ReturnsEveryField()
    {
        LEntryDraft kindle = Assert.Single(TInterface.TMarkupRead(TMarkupSample));

        Assert.Equal("kindle", kindle.LEntryDraftHeadword);
        Assert.Equal("English", kindle.LEntryDraftLanguage);
        Assert.Equal("Chiefly literary in its figurative senses.", kindle.LEntryDraftNote);
        Assert.Equal(["verb"], TInterface.TSpeechNameRead(kindle));

        LCardDraft alight = Assert.Single(kindle.LEntryDraftMeanings);
        Assert.Equal("set alight", alight.LCardDraftTitle.TStateValueShow());
        Assert.Equal("to set something burning; to start a flame", alight.LCardDraftMeaning.TStateValueShow());
        Assert.Equal(["fire"], alight.LCardDraftTag);

        Assert.Equal(
            "kindle interest",
            Assert.Single(kindle.LEntryDraftCollocations).LCardDraftExpression.TStateValueShow());
    }

    [Fact]
    public void MarkupRead_MissingHeadword_ThrowsNamingEntryIndex()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            "<llyn><entry><headword>kindle</headword></entry><entry><lang>English</lang></entry></llyn>"));

        Assert.Contains("headword", failure.Message);
        Assert.Contains("2", failure.Message);
    }

    [Fact]
    public void MarkupRead_EmptyHeadword_ThrowsLikeMissing()
    {
        Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            "<llyn><entry><headword></headword></entry></llyn>"));
    }
}
