using System.Text;
using Llyn.Conduct;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibraryRoof
{
    [Fact]
    public void LibraryMenuBuild_ImportItem_RepaintsFromTheBulletin()
    {
        List<string> asked = [];
        string? file = null;
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                file = TLibraryMarkupSave(atelier, TInterface.TMarkupLone);
                BRoofItem import = TLibraryRoofFind(library, "Import markup…");
                Assert.True(import.BRoofItemLive());
                Assert.DoesNotContain("ember", TLibrary.TLibraryDraw(library));

                import.BRoofItemAction();
                TLibraryRoofRun(() => asked.Count >= 3);

                Assert.Equal(["Markup", "Customs", "Omission"], asked);
                Assert.Contains("ember", TLibrary.TLibraryDraw(library));
            },
            envoy: TLibraryEnvoyCreate(() => file, null, asked));
    }

    [Fact]
    public void LibraryMenuBuild_ImportCancelled_AsksNothingMore()
    {
        List<string> asked = [];
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                TLibraryRoofFind(library, "Import markup…").BRoofItemAction();
                TLibraryRoofRun(() => asked.Count >= 1);

                Assert.Equal(["Markup"], asked);
                Assert.Contains("1 of 1", TLibrary.TLibraryDraw(library));
            },
            envoy: TLibraryEnvoyCreate(static () => null, null, asked));
    }

    [Fact]
    public void LibraryMenuBuild_ImportMalformed_ShowsTheImportFailure()
    {
        List<string> asked = [];
        string? file = null;
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                file = TLibraryMarkupSave(atelier, "<llyn><entry><headword>ember</headword></llyn>");

                TLibraryRoofFind(library, "Import markup…").BRoofItemAction();
                TLibraryRoofRun(() => asked.Count >= 2);

                Assert.Equal(["Markup", "List.ImportFailed>Refusal.MarkupMalformed"], asked);
                Assert.DoesNotContain("ember", TLibrary.TLibraryDraw(library));
            },
            envoy: TLibraryEnvoyCreate(() => file, null, asked));
    }

    [Fact]
    public void LibraryMenuBuild_ExportItem_LiveOnlyWhileAnEntryIsRead()
    {
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                Func<bool> live = TLibraryRoofFind(library, "Export…").BRoofItemLive;
                Assert.False(live());

                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                Assert.True(live());

                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);

                Assert.Contains(
                    library.BLibraryKeymap.BKeymapRead(BKeymapFocus.BKeymapFocusWindow),
                    static row => row.BKeymapRowLabel == "Scribe.View"
                        && row.BKeymapRowLive() == BKeymapLive.BKeymapLiveShown);
                Assert.False(live());
            });
    }

    [Fact]
    public void LibraryMenuBuild_ExportItem_WritesTheChosenFile()
    {
        List<string> asked = [];
        string? file = null;
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                file = Path.Combine(atelier.CAtelierPathRead(), "fire.md");
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);

                TLibraryRoofFind(library, "Export…").BRoofItemAction();
                TLibraryRoofRun(() => TLibraryFileCheck(file));

                Assert.Equal(["File:fire"], asked);
                Assert.Contains("fire", File.ReadAllText(file), StringComparison.Ordinal);
            },
            envoy: TLibraryEnvoyCreate(static () => null, () => file, asked));
    }

    [Fact]
    public void LibraryMenuBuild_ExportCancelled_ShowsNothing()
    {
        List<string> asked = [];
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);

                TLibraryRoofFind(library, "Export…").BRoofItemAction();
                TLibraryRoofRun(() => asked.Count >= 1);

                Assert.Equal(["File:fire"], asked);
            },
            envoy: TLibraryEnvoyCreate(static () => null, static () => null, asked));
    }

    [Fact]
    public void LibraryMenuBuild_ExportFailed_ShowsExportFailed()
    {
        List<string> asked = [];
        string? folder = null;
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                folder = atelier.CAtelierPathRead();
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);

                TLibraryRoofFind(library, "Export…").BRoofItemAction();
                TLibraryRoofRun(() => asked.Count >= 2);

                Assert.Equal("File:fire", asked[0]);
                Assert.StartsWith("Export.Failed", asked[1], StringComparison.Ordinal);
            },
            envoy: TLibraryEnvoyCreate(static () => null, () => folder, asked));
    }

    private static BRoofItem TLibraryRoofFind(BLibrary library, string label)
    {
        return library.BLibraryMenuBuild()
            .SelectMany(static menu => menu.BRoofMenuItems)
            .OfType<BRoofItem>()
            .Single(item => item.BRoofItemLabel == label);
    }

    private static void TLibraryRoofRun(Func<bool> done)
    {
        BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
        DateTime deadline = DateTime.UtcNow.AddSeconds(10);
        loop.BLoopRun(Task.Run(async () =>
        {
            while (!done() && DateTime.UtcNow < deadline)
            {
                await Task.Delay(10);
            }
        }));
    }

    private static string TLibraryMarkupSave(CAtelier atelier, string text)
    {
        string path = Path.Combine(atelier.CAtelierPathRead(), "import.llx");
        File.WriteAllText(path, text, new UTF8Encoding(false));
        return path;
    }

    private static bool TLibraryFileCheck(string path)
    {
        try
        {
            return File.Exists(path) && File.ReadAllText(path).Contains("fire", StringComparison.Ordinal);
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static CEnvoy TLibraryEnvoyCreate(Func<string?> markup, Func<string?>? file, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyMarkupRead"] = _ =>
            {
                asked.Add("Markup");
                return markup();
            },
            ["CEnvoyCustomsRead"] = _ =>
            {
                asked.Add("Customs");
                return true;
            },
            ["CEnvoyOmissionShow"] = _ =>
            {
                asked.Add("Omission");
                return null;
            },
            ["CEnvoyFileRead"] = args =>
            {
                asked.Add("File:" + (string)args![0]!);
                return (file?.Invoke(), CPortraitMedium.CPortraitMediumMarkdown);
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add(args is [string key, CLedgerNotice notice]
                    ? $"{key}>{notice.CLedgerNoticeKey}"
                    : (string)args![0]!);
                return null;
            },
        });
}
