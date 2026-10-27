using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TLectern
{
    [Fact]
    public void LecternDraw_NoEntry_ShowsPromptCentred()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, lit, _, console) =>
            {
                send(ConsoleKey.Tab);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.End);

                Assert.Null(lit());
                string prompt = "Select an entry to read it";
                Assert.Single(rows(), static row => row.Length > 0);
                (BBlueprintPart display, IReadOnlyList<(string, string?)[]> cells) = TLecternCellsRead(console);
                string line = string.Concat(cells[display.BBlueprintPartRow + (display.BBlueprintPartHeight / 2)]
                    .Skip(display.BBlueprintPartColumn)
                    .Take(display.BBlueprintPartWidth)
                    .Select(static cell => cell.Item1));
                Assert.Equal(prompt, line.Trim());
                Assert.Equal(
                    (display.BBlueprintPartWidth - prompt.Length) / 2, line.IndexOf(prompt, StringComparison.Ordinal));
            },
            height: 24,
            open: false);
    }

    [Fact]
    public void LibraryRowSelect_EnterThroughWindow_KeepsLibraryDrawn()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, _, _, console) =>
            {
                Assert.Contains(
                    console.TConsoleFakeGrid, static row => row.Contains("  fire ", StringComparison.Ordinal));

                send(ConsoleKey.Enter);

                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);
                Assert.Contains(
                    console.TConsoleFakeGrid, static row => row.StartsWith("║  fire ", StringComparison.Ordinal));
            },
            height: 24,
            open: false);
    }

    [Fact]
    public void LibraryObserve_EnterThenTab_MovesFocusBetweenSides()
    {
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                _ = save("fire", [TInterface.TCardCreate("a meaning", 1)]);
                _ = save("stone", [TInterface.TCardCreate("a meaning", 1)]);
            },
            static (send, rows, lit, _, console) =>
            {
                send(ConsoleKey.DownArrow);
                Assert.Equal(("☆", "lit"), lit());

                send(ConsoleKey.Tab);
                send(ConsoleKey.DownArrow);
                Assert.Equal(("☆", "lit-other"), lit());
                Assert.Contains(
                    console.TConsoleFakeGrid, static row => row.Contains("2 of 2", StringComparison.Ordinal));

                send(ConsoleKey.Tab);
                send(ConsoleKey.DownArrow);
                Assert.Equal(("○○○○○  Not rated", "lit"), lit());
                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);
            },
            height: 24);
    }

    [Fact]
    public void LibraryKeymapFocus_ReadingSide_KeepsKeysFromIndex()
    {
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                _ = save("fire", [TInterface.TCardCreate("a meaning", 1)]);
                _ = save("stone", [TInterface.TCardCreate("a meaning", 1)]);
            },
            static (send, rows, lit, atelier, console) =>
            {
                List<CNavigationState> states = [];
                atelier.CAtelierNavigation.CNavigationChanged += states.Add;

                send(ConsoleKey.Q);
                send(ConsoleKey.Enter);

                Assert.Empty(states);
                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);
                Assert.DoesNotContain(
                    console.TConsoleFakeGrid, static row => row.Contains("No entries match", StringComparison.Ordinal));

                send(ConsoleKey.Tab);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.Enter);

                Assert.Single(states);
                Assert.StartsWith("stone", rows()[0], StringComparison.Ordinal);
                send(ConsoleKey.DownArrow);
                Assert.Equal(("☆", "lit"), lit());
            },
            height: 24);
    }

    [Fact]
    public void LecternObserve_DownAndUp_WalksItems()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, _, lit, _, _) =>
            {
                Assert.Null(lit());

                send(ConsoleKey.DownArrow);
                Assert.Equal(("☆", "lit"), lit());

                send(ConsoleKey.DownArrow);
                Assert.Equal(("○○○○○  Not rated", "lit"), lit());

                send(ConsoleKey.DownArrow);
                Assert.Equal(("▾ Meaning", "lit"), lit());

                send(ConsoleKey.DownArrow);
                Assert.Equal(("▾ Meaning", "lit"), lit());

                send(ConsoleKey.UpArrow);
                send(ConsoleKey.UpArrow);
                Assert.Equal(("☆", "lit"), lit());

                send(ConsoleKey.Tab);
                Assert.Equal(("☆", "lit-other"), lit());
            },
            height: 24);
    }

    [Fact]
    public void LecternObserve_PageAndEndKeys_ScrollsAndLightsFirst()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save(
                "fire",
                [.. Enumerable.Range(1, 6).Select(static rank => TInterface.TCardCreate($"meaning {rank}", rank))]),
            static (send, rows, lit, _, _) =>
            {
                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);

                send(ConsoleKey.PageDown);
                Assert.DoesNotContain(rows(), static row => row.StartsWith("fire", StringComparison.Ordinal));
                (string, string)? first = lit();
                Assert.Equal("lit", first?.Item2);
                Assert.StartsWith(
                    first!.Value.Item1,
                    rows().First(static row => row.Length > 0 && row != "Meanings").TrimStart("123456 ".ToCharArray()),
                    StringComparison.Ordinal);

                send(ConsoleKey.Home);
                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);
                Assert.Equal(("☆", "lit"), lit());

                send(ConsoleKey.End);
                IReadOnlyList<string> foot = rows();
                Assert.Contains(foot, static row => row.StartsWith("Created", StringComparison.Ordinal));
                Assert.Contains(foot, static row => row.StartsWith("Updated", StringComparison.Ordinal));
                Assert.Equal("lit", lit()?.Item2);

                send(ConsoleKey.PageUp);
                send(ConsoleKey.PageUp);
                send(ConsoleKey.PageUp);
                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);
                Assert.Equal(("☆", "lit"), lit());
            },
            height: 24);
    }

    [Fact]
    public void LecternDraw_FrequencyFails_NoticesOnceAndReadsRest()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, _, _, console) =>
            {
                IReadOnlyList<string> shown = rows();
                send(ConsoleKey.DownArrow);
                _ = rows();

                Assert.Equal(1, TLecternNoticeRead(console, "The frequency could not be read."));
                Assert.StartsWith("fire", shown[0], StringComparison.Ordinal);
                Assert.StartsWith("English", shown[1], StringComparison.Ordinal);
                Assert.Equal("○○○○○  Not rated", shown[2]);
            },
            "ALTER TABLE frequency RENAME TO frequency_gone;",
            24,
            notice: "could not be");
    }

    [Fact]
    public void LecternDraw_EntryEditedElsewhere_ShowsNewHeadword()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (_, rows, _, atelier, _) =>
            {
                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);

                CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, []));
                editor.CEditorEntryOpen(1);
                editor.CEditorEntry.CEntryHeadwordSet("flame");
                Assert.True(editor.CEditorEntrySave());

                Assert.StartsWith("flame", rows()[0], StringComparison.Ordinal);
            },
            height: 24);
    }

    [Fact]
    public void LibraryRowSelect_OpenFails_KeepsFocusOnIndex()
    {
        int closes = TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, lit, _, console) =>
            {
                Assert.DoesNotContain(rows(), static row => row.StartsWith("fire", StringComparison.Ordinal));
                Assert.Contains(console.TConsoleFakeGrid, static row => row.Contains("fire", StringComparison.Ordinal));

                send(ConsoleKey.Enter);
                send(ConsoleKey.DownArrow);

                Assert.Contains("Select an entry to read it", rows());
                Assert.Null(lit());
                Assert.Equal(1, TLecternNoticeRead(console, "The entry could not be opened."));
            },
            "ALTER TABLE entry RENAME TO entry_gone;",
            24,
            false,
            "could not be");

        Assert.Equal(1, closes);
    }

    internal static int TLecternNoticeRead(TConsoleFake console, string notice)
    {
        bool[] shown =
        [
            .. console.TConsoleFakeShots.Select(shot =>
                shot.Any(row => row.Contains(notice, StringComparison.Ordinal))),
        ];
        return shown.Where((up, index) => up && (index == 0 || !shown[index - 1])).Count();
    }

    internal static Func<int> TLecternNoticeClose(TConsoleFake console, string notice, BLoop loop, Action batch)
    {
        int sent = 0;
        Func<bool> shown = () =>
        {
            try
            {
                return console.TConsoleFakeGrid.Any(row => row.Contains(notice, StringComparison.Ordinal));
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        };
        _ = Task.Run(() =>
        {
            DateTime end = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < end)
            {
                if (SpinWait.SpinUntil(shown, TimeSpan.FromSeconds(1)))
                {
                    _ = Interlocked.Increment(ref sent);
                    console.TConsoleFakeSettle(
                        TLecternKeyCreate(ConsoleKey.Escape), loop, batch, CancellationToken.None);
                }
            }
        });
        return () => Volatile.Read(ref sent);
    }

    internal static string? TLecternRoleRead(TConsoleFake console, string text)
    {
        (BBlueprintPart display, IReadOnlyList<(string, string?)[]> cells) = TLecternCellsRead(console);
        foreach ((string, string?)[] row in cells.Skip(display.BBlueprintPartRow).Take(display.BBlueprintPartHeight))
        {
            (string, string?)[] part = [.. row.Skip(display.BBlueprintPartColumn).Take(display.BBlueprintPartWidth)];
            int at = string.Concat(part.Select(static cell => cell.Item1)).IndexOf(text, StringComparison.Ordinal);
            if (at >= 0)
            {
                return part[at].Item2;
            }
        }

        return null;
    }

    internal static ConsoleKeyInfo TLecternKeyCreate(ConsoleKey key)
    {
        char typed = key switch
        {
            ConsoleKey.Enter => '\r',
            ConsoleKey.Tab => '\t',
            >= ConsoleKey.A and <= ConsoleKey.Z => (char)('a' + (key - ConsoleKey.A)),
            _ => '\0',
        };
        return new ConsoleKeyInfo(typed, key, false, false, false);
    }

    internal static (BBlueprintPart, IReadOnlyList<(string, string?)[]>) TLecternCellsRead(TConsoleFake console)
    {
        BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, console.TConsoleFakeWidth, console.TConsoleFakeHeight);
        BBlueprintPart display = BContract.BContractFind(window, "ADisplay");
        BCanvasWidth measure = new(console.TConsoleFakeAmbiguous);
        string?[][] roles = console.TConsoleFakeRoles;
        List<(string, string?)[]> rows = [];
        foreach ((string line, int row) in console.TConsoleFakeGrid.Select(static (line, row) => (line, row)))
        {
            (string, string?)[] cells = new (string, string?)[console.TConsoleFakeWidth];
            Array.Fill(cells, (" ", null));
            int column = 0;
            System.Globalization.TextElementEnumerator walk =
                System.Globalization.StringInfo.GetTextElementEnumerator(line);
            while (walk.MoveNext() && column < cells.Length)
            {
                string cluster = walk.GetTextElement();
                cells[column] = (cluster, roles[row][column]);
                int width = measure.BCanvasWidthResolve(cluster);
                if (width == 2 && column + 1 < cells.Length)
                {
                    cells[column + 1] = (string.Empty, roles[row][column + 1]);
                }

                column += width;
            }

            rows.Add(cells);
        }

        return (display, rows);
    }

    internal static IReadOnlyList<string> TLecternRowsRead(TConsoleFake console)
    {
        (BBlueprintPart display, IReadOnlyList<(string, string?)[]> rows) = TLecternCellsRead(console);
        int page = display.BBlueprintPartHeight - 1;
        return
        [
            .. Enumerable.Range(display.BBlueprintPartRow, page).Select(row => string.Concat(rows[row]
                .Skip(display.BBlueprintPartColumn)
                .Take(display.BBlueprintPartWidth)
                .Select(static cell => cell.Item1)).Trim()),
        ];
    }

    internal static (string, string)? TLecternLitRead(TConsoleFake console)
    {
        (BBlueprintPart display, IReadOnlyList<(string, string?)[]> rows) = TLecternCellsRead(console);
        List<(string, string)> runs = [];
        for (int row = display.BBlueprintPartRow; row < display.BBlueprintPartRow + display.BBlueprintPartHeight; row++)
        {
            string? role = null;
            System.Text.StringBuilder text = new();
            foreach ((string cluster, string? tint) in rows[row]
                .Skip(display.BBlueprintPartColumn)
                .Take(display.BBlueprintPartWidth)
                .Append((string.Empty, null)))
            {
                if (tint != role && role is "lit" or "lit-other")
                {
                    runs.Add((text.ToString().Trim(), role));
                }

                if (tint != role)
                {
                    _ = text.Clear();
                    role = tint;
                }

                _ = text.Append(cluster);
            }
        }

        return runs.Count == 0 ? null : Assert.Single(runs);
    }
}
