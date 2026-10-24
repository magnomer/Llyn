using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TPathfinderMedium
{
    private static readonly ConsoleKeyInfo TPathfinderMediumEnter = new('\r', ConsoleKey.Enter, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderMediumEscape = new('\e', ConsoleKey.Escape, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderMediumRight =
        new('\0', ConsoleKey.RightArrow, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderMediumLeft = new('\0', ConsoleKey.LeftArrow, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderMediumDown = new('\0', ConsoleKey.DownArrow, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderMediumTab = new('\t', ConsoleKey.Tab, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderMediumBack = new('\t', ConsoleKey.Tab, true, false, false);
    private static readonly ConsoleKeyInfo[] TPathfinderMediumErase =
        [.. Enumerable.Repeat(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false), 5)];
    private static readonly IReadOnlyList<CPortraitChoice> TPathfinderMediumChoices =
    [
        new("Export.Markup", ".llx", true, CPortraitMedium.CPortraitMediumMarkup),
        new("Export.Html", ".html", false, CPortraitMedium.CPortraitMediumHtml),
    ];

    [Fact]
    public void PathfinderSave_FieldEnter_SavesOfferedName()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [], _ => [TPathfinderMediumEnter], TPathfinderMediumConsult, out var shots);

        Assert.Equal("~\\entry.llx|CPortraitMediumMarkup", answer);
        Assert.Contains(shots, static shot => shot.Contains(" Export entry ", StringComparison.Ordinal));
        Assert.Contains(shots, static shot => shot.Contains("[ Save ]", StringComparison.Ordinal));
        Assert.Contains(shots, static shot => shot.Contains("Format Llyn Markup", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderSave_Right_ChangesFormatAndSuffix()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [], _ => [TPathfinderMediumTab, TPathfinderMediumRight, TPathfinderMediumEnter], TPathfinderMediumConsult,
            out var shots);

        Assert.Equal("~\\entry.html|CPortraitMediumHtml", answer);
        Assert.Contains(shots, static shot => shot.Contains("Format HTML", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderSave_FieldRight_KeepsFormat()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [], _ => [TPathfinderMediumRight, TPathfinderMediumEnter], TPathfinderMediumConsult, out _);

        Assert.Equal("~\\entry.llx|CPortraitMediumMarkup", answer);
    }

    [Fact]
    public void PathfinderSave_TabTab_ReachesList()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["a.llx"],
            _ => [TPathfinderMediumTab, TPathfinderMediumTab, TPathfinderMediumDown, TPathfinderMediumEnter,
                TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\a.llx|CPortraitMediumMarkup", answer);
    }

    [Fact]
    public void PathfinderSave_ShiftTabFromList_PassesFormatToField()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [TPathfinderMediumDown, TPathfinderMediumBack, TPathfinderMediumRight, TPathfinderMediumBack,
                TPathfinderMediumRight, TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\entry.html|CPortraitMediumHtml", answer);
    }

    [Fact]
    public void PathfinderSave_TabToFormat_DrawsFormatLit()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BSPathfinder pathfinder = new(
            new ABootstrap().BBlueprintParts["ASPathfinder"], catalog, "C:\\", null, static _ => ([], []),
            static _ => false, static _ => { }, static () => null, new BSPathfinderMedium(TPathfinderMediumChoices));
        Func<string?> format = () => pathfinder.BSParleyBodyDraw(
                new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 72, 24, true, "dialog", new BCanvasWidth(false))
            .First(static text => text.BCanvasTextValue.StartsWith("Format", StringComparison.Ordinal))
            .BCanvasTextRole;
        Assert.NotEqual("lit", format());

        pathfinder.BSParleyBodyObserve(TPathfinderMediumTab);

        Assert.Equal("lit", format());
    }

    [Fact]
    public void PathfinderSave_TypeAfterRight_AppendsAtEnd()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [TPathfinderMediumTab, TPathfinderMediumRight, TPathfinder.TPathfinderKeyBuild('x'),
                TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out var shots);

        Assert.Equal("~\\entryx.html|CPortraitMediumHtml", answer);
        Assert.Contains(shots, static shot => shot.Contains("entryx", StringComparison.Ordinal));
        Assert.DoesNotContain(shots, static shot => shot.Contains("entry.html", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderSave_TypedSuffix_KeepsName()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [.. ".LLX".Select(TPathfinder.TPathfinderKeyBuild), TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\entry.LLX|CPortraitMediumMarkup", answer);
    }

    [Fact]
    public void PathfinderSave_RightLeft_KeepsName()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ =>
            [
                TPathfinder.TPathfinderKeyBuild('x'), TPathfinderMediumTab, TPathfinderMediumRight,
                TPathfinderMediumLeft, TPathfinderMediumRight, TPathfinderMediumEnter,
            ],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\entryx.html|CPortraitMediumHtml", answer);
    }

    [Fact]
    public void PathfinderSave_TypedNameWithoutSuffix_AppendsSuffix()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [.. TPathfinderMediumErase, TPathfinder.TPathfinderKeyBuild('a'), TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\a.llx|CPortraitMediumMarkup", answer);
    }

    [Fact]
    public void PathfinderSave_EmptyFieldEnter_AsksNothing()
    {
        string? sibling = null;
        try
        {
            string? answer = TPathfinder.TPathfinderRun(
                [],
                _ =>
                [
                    .. TPathfinderMediumErase, TPathfinderMediumEnter, TPathfinder.TPathfinderKeyBuild('x'),
                    TPathfinderMediumEnter,
                ],
                (envoy, root) =>
                {
                    sibling = root + ".llx";
                    File.WriteAllText(sibling, string.Empty);
                    return TPathfinderMediumConsult(envoy, root);
                },
                out var shots);

            Assert.Equal("~\\x.llx|CPortraitMediumMarkup", answer);
            Assert.DoesNotContain(shots, static shot => shot.Contains("Replace it?", StringComparison.Ordinal));
        }
        finally
        {
            if (sibling is not null)
            {
                File.Delete(sibling);
            }
        }
    }

    [Fact]
    public void PathfinderSave_TypedNameWithoutSuffixExisting_AsksReplace()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["entry.llx"],
            _ =>
            [
                .. TPathfinderMediumErase, .. "entry".Select(TPathfinder.TPathfinderKeyBuild), TPathfinderMediumEnter,
                TPathfinderMediumEnter,
            ],
            TPathfinderMediumConsult,
            out var shots);

        Assert.Equal("~\\entry.llx|CPortraitMediumMarkup", answer);
        Assert.Contains(shots, static shot => shot.Contains("Replace it?", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderSave_Right_ListsOnlyFormatFiles()
    {
        TPathfinder.TPathfinderRun(
            ["a.llx", "b.html"],
            _ => [TPathfinderMediumTab, TPathfinderMediumRight, TPathfinderMediumEscape],
            TPathfinderMediumConsult,
            out var shots);

        Assert.Contains("b.html", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("a.llx", shots[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void PathfinderSave_RightWithTypedFolder_ListsThatFolder()
    {
        TPathfinder.TPathfinderRun(
            ["b\\", "b\\x.html", "y.html"],
            _ => [.. TPathfinderMediumErase, .. "b\\".Select(TPathfinder.TPathfinderKeyBuild), TPathfinderMediumTab,
                TPathfinderMediumRight, TPathfinderMediumEscape],
            TPathfinderMediumConsult,
            out var shots);

        Assert.Contains("x.html", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("y.html", shots[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void PathfinderSave_TypedFolderRowEnter_ClearsField()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["a\\", "a\\w\\"],
            _ => [.. TPathfinderMediumErase, .. "a\\w".Select(TPathfinder.TPathfinderKeyBuild), TPathfinderMediumDown,
                TPathfinderMediumEnter, TPathfinder.TPathfinderKeyBuild('n'), TPathfinderMediumBack,
                TPathfinderMediumBack, TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\a\\w\\n.llx|CPortraitMediumMarkup", answer);
    }

    [Fact]
    public void PathfinderSave_NameFolderRowEnter_KeepsName()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["b\\"],
            _ => [TPathfinderMediumDown, TPathfinderMediumDown, TPathfinderMediumEnter, TPathfinderMediumBack,
                TPathfinderMediumBack, TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\b\\entry.llx|CPortraitMediumMarkup", answer);
    }

    [Fact]
    public void PathfinderSave_Escape_AnswersNullWithCurrentMedium()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [], _ => [TPathfinderMediumTab, TPathfinderMediumRight, TPathfinderMediumEscape], TPathfinderMediumConsult,
            out _);

        Assert.Equal("|CPortraitMediumHtml", answer);
    }

    [Fact]
    public void PathfinderSave_ExistingFileYes_Replaces()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["entry.llx"],
            _ => [TPathfinderMediumEnter, TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out var shots);

        Assert.Equal("~\\entry.llx|CPortraitMediumMarkup", answer);
        Assert.Contains(shots, static shot => shot.Contains("Replace it?", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderSave_ExistingFileNo_Reopens()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["entry.llx"],
            _ =>
            [
                TPathfinderMediumEnter, TPathfinderMediumEscape, TPathfinderMediumTab, TPathfinderMediumRight,
                TPathfinderMediumEnter,
            ],
            TPathfinderMediumConsult,
            out _);

        Assert.Equal("~\\entry.html|CPortraitMediumHtml", answer);
    }

    [Fact]
    public void PathfinderSave_ListEnterOnFile_AsksReplace()
    {
        string? answer = TPathfinder.TPathfinderRun(
            ["a.llx"],
            _ => [TPathfinderMediumDown, TPathfinderMediumDown, TPathfinderMediumEnter, TPathfinderMediumEnter],
            TPathfinderMediumConsult,
            out var shots);

        Assert.Equal("~\\a.llx|CPortraitMediumMarkup", answer);
        Assert.Contains(shots, static shot => shot.Contains("Replace it?", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderMediumRemove_KnownSuffix_StripsIt()
    {
        BSPathfinderMedium medium = new(
        [
            new("Export.Html", ".html", false, CPortraitMedium.CPortraitMediumHtml),
            new("Export.Markup", ".b.html", true, CPortraitMedium.CPortraitMediumMarkup),
        ]);

        Assert.Equal("a", medium.BSPathfinderMediumRemove("a.B.HTML"));
        Assert.Equal("a.llx", medium.BSPathfinderMediumRemove("a.llx"));
        Assert.Equal(string.Empty, medium.BSPathfinderMediumRemove(string.Empty));
    }

    [Fact]
    public void PathfinderMediumMove_PastEnds_Clamps()
    {
        BSPathfinderMedium medium = new(TPathfinderMediumChoices);

        medium.BSPathfinderMediumMove(-1);
        Assert.Equal(CPortraitMedium.CPortraitMediumMarkup, medium.BSPathfinderMediumChoice.CPortraitChoiceMedium);
        medium.BSPathfinderMediumMove(5);
        Assert.Equal(CPortraitMedium.CPortraitMediumHtml, medium.BSPathfinderMediumChoice.CPortraitChoiceMedium);
    }

    private static string? TPathfinderMediumConsult(BEnvoy envoy, string root)
    {
        (string? file, CPortraitMedium medium) =
            envoy.CEnvoyFileRead(Path.Combine(root, "entry.llx"), TPathfinderMediumChoices);
        return $"{file}|{medium}";
    }
}
