using Llyn.Conduct;
using Llyn.Core;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternCompass
{
    [Fact]
    public void LecternCompassShow_AltF3_ListsTheRowsAndLightsTheFirstAtTheTop()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 2),
            static (send, _, lit, _, console) =>
            {
                (List<string> box, string? shown) = TLecternCompassSend(console, send, ConsoleKey.Escape);

                Assert.Contains(box, static row => row.Contains("Contents", StringComparison.Ordinal));
                Assert.Contains(box, static row => row.Contains("Meanings", StringComparison.Ordinal));
                Assert.Contains(box, static row => row.Contains("  1  sense1", StringComparison.Ordinal));
                Assert.Contains(box, static row => row.Contains("  2  sense2", StringComparison.Ordinal));
                Assert.Equal("Meanings", shown);
                Assert.Null(lit());
            });
    }

    [Fact]
    public void LecternCompassShow_ScrolledToMeaningFive_LightsItsRow()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 12),
            static (send, rows, _, _, console) =>
            {
                send(ConsoleKey.D5);
                Assert.EndsWith("sense5", rows()[0], StringComparison.Ordinal);

                (_, string? shown) = TLecternCompassSend(console, send, ConsoleKey.Escape);

                Assert.Equal("5  sense5", shown);
            },
            height: 24);
    }

    [Fact]
    public void LecternCompassObserve_DownThenEnter_ScrollsAndLightsTheCard()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 12),
            static (send, rows, lit, _, console) =>
            {
                string? shown = TLecternCompassSend(
                    console, send, ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.Enter).Item2;

                Assert.Equal("Meanings", shown);
                Assert.Equal(("▾ sense6", "lit"), lit());
                Assert.EndsWith("sense6", rows()[0], StringComparison.Ordinal);
            },
            height: 24);
    }

    [Fact]
    public void LecternCompassObserve_Escape_ClosesAndScrollsNothing()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 8),
            static (send, rows, lit, _, console) =>
            {
                List<string> box = [];
                send(ConsoleKey.D3);
                IReadOnlyList<string> before = rows();

                box.AddRange(TLecternCompassSend(console, send, ConsoleKey.DownArrow, ConsoleKey.Escape).Item1);

                Assert.NotEmpty(box);
                Assert.Equal(before, rows());
                Assert.Equal(("▾ sense3", "lit"), lit());
                Assert.DoesNotContain(console.TConsoleFakeGrid, static row =>
                    row.Contains("Contents", StringComparison.Ordinal));
            },
            height: 24);
    }

    [Fact]
    public void LecternCompassShow_HiddenParts_ListsOnlyTheShownParts()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 1),
            static (send, _, _, _, console) =>
            {
                (List<string> box, _) = TLecternCompassSend(console, send, ConsoleKey.Escape);

                Assert.Contains(box, static row => row.Contains("Meanings", StringComparison.Ordinal));
                Assert.DoesNotContain(box, static row => row.Contains("Other collocations", StringComparison.Ordinal));
                Assert.DoesNotContain(box, static row => row.Contains("Note", StringComparison.Ordinal));
                Assert.DoesNotContain(box, static row => row.Contains("Translated", StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternCompassShow_OtherCollocation_ListsItsRow()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "apple",
                "English",
                string.Empty,
                string.Empty,
                [TInterface.TCardCreate("a round fruit", 1) with { LCardDraftTitle = "fruit" }],
                [
                    TInterface.TCardDraftCreate(
                        TInterfaceState.TStateValueCreate("eat apples"),
                        TInterfaceState.TStateValueCreate("eat apples"),
                        LStateValue.LStateValueUnspecified,
                        [], [], [], [], [], 1),
                ])),
            static (send, _, _, _, console) =>
            {
                (List<string> box, _) = TLecternCompassSend(console, send, ConsoleKey.Escape);

                Assert.Contains(box, static row => row.Contains("Other collocations", StringComparison.Ordinal));
                Assert.Contains(box, static row => row.Contains("  1  eat apples", StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternWholeObserve_F3ThenF3_GivesTheWindowAndTheListBack()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 2),
            static (send, rows, lit, _, console) =>
            {
                Assert.NotEmpty(rows());
                Assert.False(TLecternCompassCheck(console));

                send(ConsoleKey.F3);

                Assert.True(TLecternCompassCheck(console));
                send(ConsoleKey.DownArrow);
                Assert.NotNull(lit());

                send(ConsoleKey.F3);

                Assert.False(TLecternCompassCheck(console));
            });
    }

    [Fact]
    public void LecternWholeObserve_Escape_GivesTheListBack()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => TLecternCompassSave(save, 2),
            static (send, rows, _, _, console) =>
            {
                send(ConsoleKey.F3);
                Assert.True(TLecternCompassCheck(console));

                send(ConsoleKey.Escape);

                _ = rows();
                Assert.False(TLecternCompassCheck(console));
            });
    }

    [Fact]
    public void LecternSeeObserve_CardFarAbove_ScrollsToItsCardAndLightsTheHeader()
    {
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                LCardDraft first = TInterface.TCardCreate("to place firmly", 1) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                    LCardDraftTitle = "first",
                };
                LCardDraft[] fillers =
                [
                    .. Enumerable.Range(2, 8).Select(static index =>
                        TInterface.TCardCreate("definition " + index, index) with
                        {
                            LCardDraftTitle = "sense" + index,
                        }),
                ];
                LCardDraft other = TInterface.TCardCreate("to harden", 10) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                };
                LCardDraft linked = TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("set off"),
                    LStateValue.LStateValueUnspecified,
                    [], [], [], [], [], 1) with
                {
                    LCardDraftSense = [first.LCardDraftId, other.LCardDraftId],
                };
                _ = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                    "set", "English", string.Empty, string.Empty, [first, .. fillers, other], [linked]));
            },
            static (send, rows, lit, _, _) =>
            {
                send(ConsoleKey.End);
                for (int step = 0; step < 40 && lit()?.Item1.StartsWith("→ set off", StringComparison.Ordinal) != true;
                    step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.StartsWith("→ set off", lit()?.Item1, StringComparison.Ordinal);
                Assert.DoesNotContain(rows(), static row => row.EndsWith("first", StringComparison.Ordinal));

                send(ConsoleKey.Enter);

                Assert.EndsWith("first", rows()[0], StringComparison.Ordinal);
                Assert.Equal(("▾ set off", "lit"), lit());
            },
            height: 24);
    }

    private static void TLecternCompassSave(Func<string, IReadOnlyList<LCardDraft>, long> save, int count)
    {
        _ = save(
            "apple",
            [
                .. Enumerable.Range(1, count).Select(static index =>
                    TInterface.TCardCreate("definition " + index, index) with { LCardDraftTitle = "sense" + index }),
            ]);
    }

    private static bool TLecternCompassCheck(TConsoleFake console)
    {
        string[] grid = console.TConsoleFakeGrid;
        return grid.Any(static row => row.Contains("apple", StringComparison.Ordinal))
            && grid.Skip(2).Take(grid.Length - 7).All(static row => row.IndexOf('║', 1) == row.LastIndexOf('║'));
    }

    private static (List<string>, string?) TLecternCompassSend(
        TConsoleFake console, Action<ConsoleKey> send, params ConsoleKey[] keys)
    {
        List<string> box = [];
        string? shown = null;
        Task sent = Task.Run(() =>
        {
            string[] grid = [];
            string?[][] roles = [];
            Func<bool> open = () =>
            {
                try
                {
                    grid = [.. console.TConsoleFakeGrid];
                    roles = console.TConsoleFakeRoles;
                    return grid.Any(static row => row.Contains(" Contents ", StringComparison.Ordinal));
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            };
            Assert.True(SpinWait.SpinUntil(open, TimeSpan.FromSeconds(30)));
            box.AddRange(grid);
            int title = Array.FindIndex(grid, static row => row.Contains(" Contents ", StringComparison.Ordinal));
            int left = grid[title].IndexOf('╔', StringComparison.Ordinal);
            for (int row = title + 1; row < grid.Length && shown is null; row++)
            {
                int right = Math.Min(grid[row].Length, roles[row].Length);
                int[] cells = [.. Enumerable.Range(left, Math.Max(right - left, 0))
                    .Where(column => roles[row][column] == "lit")];
                shown = cells.Length == 0 ? null : grid[row][cells[0]..(cells[^1] + 1)].Trim();
            }

            foreach (ConsoleKey key in keys)
            {
                console.TConsoleFakeKeys.Add(TLectern.TLecternKeyCreate(key));
            }

            Assert.True(SpinWait.SpinUntil(() => !open(), TimeSpan.FromSeconds(30)));
        });
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F3, false, true, false));
        ((BLoop)SynchronizationContext.Current!).BLoopRun(sent);
        send(ConsoleKey.F24);
        sent.GetAwaiter().GetResult();
        return (box, shown);
    }
}
