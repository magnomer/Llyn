using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupImport
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
              <note></note>
            </source>

            <example id="ex-brush" lang="English" src="oed">
              <text>he kindled the dry brush with a single match</text>
              <trans>그는 성냥 하나로 마른 덤불에 불을 붙였다</trans>
            </example>

            <situation id="hearth">
              <title>Around a hearth</title>
            </situation>

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

            <sense id="s-alight">
              <title>set alight</title>
              <meaning>to set something burning; to start a flame</meaning>
              <tag>literal</tag>
              <use ref="ex-brush"/>
            </sense>
          </entry>

          <entry id="brook">
            <headword>brook</headword>
            <lang>English</lang>

            <sense id="s-stream">
              <meaning>a small natural watercourse</meaning>
              <tag>nature</tag>
            </sense>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupImport_Catalog_StoresRowsPointingAtEachOther()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEntry> imported = engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupSample));

        Assert.Equal(["kindle", "brook"], imported.Select(entry => entry.LEntryHeadword));

        LReference oed = Assert.Single(engine.TEngineReferenceRead());
        Assert.Equal("Oxford English Dictionary", oed.LReferenceTitle.TStateValueShow());
        Assert.Equal("1928", oed.LReferenceYear.TStateValueShow());
        Assert.Equal(LReferenceKind.LReferenceKindBook, oed.LReferenceKind);
        Assert.Equal(LState.LStateUnknown, oed.LReferenceNote.LStateValueState);
        Assert.Equal(LState.LStateSpecified, oed.LReferenceAuthorState);
        Assert.Equal(
            ["Murray, James", "Bradley, Henry"],
            engine.TEngineAuthorRead(oed.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));

        LExample brush = Assert.Single(engine.TEngineExampleRead());
        Assert.Equal("English", brush.LExampleLanguage);
        Assert.Equal("he kindled the dry brush with a single match", brush.LExampleText.TStateValueShow());
        Assert.Equal("그는 성냥 하나로 마른 덤불에 불을 붙였다", brush.LExampleTranslation.TStateValueShow());
        Assert.Equal(oed.LReferenceId, brush.LExampleSource.TStateValueShow());

        Assert.Equal("Around a hearth", Assert.Single(engine.TEngineSituationRead()).LSituationTitle.TStateValueShow());
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM video;"));
    }

    [Fact]
    public void MarkupImport_KeyOfARow_LeavesNoTraceInTheStore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(TMarkupSample));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source WHERE id = 'oed';"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example WHERE id = 'ex-brush';"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry WHERE id = 'kindle';"));
    }

    [Fact]
    public void MarkupImport_UncitedSource_StoresItAnyway()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(
            """
            <llyn>
              <catalog>
                <source id="dormant"><title>A Work Nothing Cites</title></source>
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
            </llyn>
            """));

        Assert.Equal(
            "A Work Nothing Cites",
            Assert.Single(engine.TEngineReferenceRead()).LReferenceTitle.TStateValueShow());
    }

    [Fact]
    public void MarkupImport_UncreditedAuthor_StoresItAnyway()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(
            """
            <llyn>
              <catalog>
                <author id="gaskell">Gaskell, Ruth</author>
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
            </llyn>
            """));

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM author;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source_author;"));
    }

    [Fact]
    public void MarkupImport_TwoAuthorsOneName_StoresTwoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(
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
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
            </llyn>
            """));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM author;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source_author;"));
    }

    [Fact]
    public void MarkupImport_KeyDeclaredTwice_StoresNothing()
    {
        TMarkupRefusalCheck(
            """
            <llyn>
              <catalog>
                <source id="oed"><title>Oxford English Dictionary</title></source>
                <source id="oed"><title>A Field Guide to Rivers</title></source>
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
            </llyn>
            """);
    }

    [Fact]
    public void MarkupImport_DanglingReference_StoresNothing()
    {
        TMarkupRefusalCheck(
            """
            <llyn>
              <catalog>
                <example id="ex1" lang="English" src="oed"><text>she kindled the lamp</text></example>
              </catalog>
              <entry><headword>kindle</headword><lang>English</lang></entry>
            </llyn>
            """);
    }

    [Fact]
    public void MarkupImport_CitationOfAWrongKind_StoresNothing()
    {
        TMarkupRefusalCheck(
            """
            <llyn>
              <catalog>
                <source id="oed"><title>Oxford English Dictionary</title></source>
              </catalog>
              <entry>
                <headword>kindle</headword>
                <lang>English</lang>
                <sense><situation ref="oed"/></sense>
              </entry>
            </llyn>
            """);
    }

    [Fact]
    public void MarkupImport_FileWrittenBefore_StoresNothing()
    {
        TMarkupRefusalCheck(
            """
            <entry>
              <headword>kindle</headword>
              <lang>English</lang>
              <source id="oed"><title>Oxford English Dictionary</title></source>
            </entry>
            """);
    }

    [Fact]
    public void MarkupImport_PathThatDoesNotExist_ImportsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = Path.Combine(workspace.TWorkspaceFolder, "absent.llx");

        Assert.Throws<FileNotFoundException>(() => engine.TEngineMarkupImport(path));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    private static void TMarkupRefusalCheck(string text)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Throws<FormatException>(
            () => engine.TEngineMarkupImport(workspace.TWorkspaceMarkupSave(text)));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM source;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM author;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }
}
