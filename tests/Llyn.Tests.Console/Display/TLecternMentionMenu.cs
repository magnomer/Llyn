using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternMentionMenu
{
    [Fact]
    public void LecternMentionFind_ManyAnswers_OpensTheMenuUnderTheWordAndOpensThePick()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMenuSave(engine),
            static (send, rows, lit, _, console) =>
            {
                TLecternMention.TLecternMentionSeek(send, rows, console, TLecternMention.TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMention.TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                send(ConsoleKey.RightArrow);
                List<string> menu = [];
                TLecternMenuSettle(console, menu, ConsoleKey.DownArrow, ConsoleKey.Enter);

                send(ConsoleKey.Enter);

                int line = menu.FindIndex(static row =>
                    row.Contains(TLecternMention.TLecternMentionSentence, StringComparison.Ordinal));
                Assert.True(line >= 0);
                int word = menu[line].IndexOf("water", StringComparison.Ordinal);
                Assert.Equal('╔', menu[line + 1][word]);
                Assert.Contains("Which entry?", menu[line + 1], StringComparison.Ordinal);
                Assert.Equal(2, menu.Count(static row => row.Contains("water  English", StringComparison.Ordinal)));
                Assert.StartsWith("water", rows()[0], StringComparison.Ordinal);
                Assert.Contains("to pour", rows());
                Assert.DoesNotContain("liquid", rows());
                Assert.DoesNotContain(console.TConsoleFakeGrid, static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternMentionFind_MenuEscape_ClosesTheMenuAndKeepsTheCursor()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMenuSave(engine),
            static (send, rows, lit, _, console) =>
            {
                TLecternMention.TLecternMentionSeek(send, rows, console, TLecternMention.TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMention.TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                List<string> menu = [];
                TLecternMenuSettle(console, menu, ConsoleKey.Escape);

                send(ConsoleKey.Enter);

                Assert.NotEmpty(menu);
                Assert.Equal(("w", "lit"), lit());
                Assert.Contains(rows(), static row =>
                    row.EndsWith(TLecternMention.TLecternMentionSentence, StringComparison.Ordinal));
                Assert.DoesNotContain(console.TConsoleFakeGrid, static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal));
                Assert.True(TLecternMention.TLecternMentionCheck(console));
            });
    }

    [Fact]
    public void LecternMentionFind_Landing_ClosesTheMenu()
    {
        long landing = 0;
        TLecternCard.TLecternCardRun(
            (engine, _) =>
            {
                landing = engine.TEngineEntrySave(
                    TLecternMention.TLecternMentionCreate("water", [], "liquid")).LEntryId;
                engine.TEngineEntrySave(TLecternMention.TLecternMentionCreate("water", [], "to pour"));
                TLecternMention.TLecternMentionSave(engine, TLecternMention.TLecternMentionSentence, []);
            },
            (send, rows, lit, atelier, console) =>
            {
                TLecternMention.TLecternMentionSeek(send, rows, console, TLecternMention.TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMention.TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                SynchronizationContext loop = SynchronizationContext.Current!;
                List<string> menu = [];
                TLecternMenuSettle(console, menu);
                _ = Task.Run(() =>
                {
                    if (SpinWait.SpinUntil(() => menu.Count > 0, TimeSpan.FromSeconds(30)))
                    {
                        loop.Post(_ => atelier.CAtelierNavigation.CNavigationEntryOpen(landing), null);
                    }
                });

                send(ConsoleKey.Enter);

                Assert.NotEmpty(menu);
                Assert.StartsWith("water", rows()[0], StringComparison.Ordinal);
                Assert.Contains("liquid", rows());
                Assert.DoesNotContain(console.TConsoleFakeGrid, static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternMentionFind_NoEntry_OpensNoMenuAndKeepsTheCursor()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
                TLecternMention.TLecternMentionSave(engine, TLecternMention.TLecternMentionSentence, []),
            static (send, rows, lit, _, console) =>
            {
                TLecternMention.TLecternMentionSeek(send, rows, console, TLecternMention.TLecternMentionSentence);
                send(ConsoleKey.Enter);

                send(ConsoleKey.Enter);

                Assert.Equal(("T", "lit"), lit());
                Assert.DoesNotContain(console.TConsoleFakeShots, static shot => shot.Any(static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal)));
            });
    }

    [Fact]
    public void MentionMenuShow_EmptyOffer_OpensNothing()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
                TLecternMention.TLecternMentionSave(engine, TLecternMention.TLecternMentionSentence, []),
            static (_, _, _, atelier, _) =>
            {
                int opened = 0;
                Func<BSParley, Task?, int?> docket = (_, _) =>
                {
                    opened++;
                    return null;
                };
                BBlueprintPart blueprint = new ABootstrap().BBlueprintParts["ASParley"];
                CMentionOffer empty = new(0, [], "Mention.Title");

                foreach (CMentionOffer? offer in new[] { null, empty })
                {
                    BSMentionMenu.BSMentionMenuShow(
                        new BLocalizationCatalog(), blueprint, docket, atelier.CAtelierNavigation, offer, (1, 0),
                        new BCanvasWidth(false));
                }

                Assert.Equal(0, opened);
            });
    }

    [Fact]
    public void LecternMentionFind_ManyAnswers_FirstRowStartsLitAndEnterOpensIt()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMenuSave(engine),
            static (send, rows, lit, _, console) =>
            {
                TLecternMention.TLecternMentionSeek(send, rows, console, TLecternMention.TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMention.TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                List<int> lits = [];
                TLecternMenuSend(console, lits);

                send(ConsoleKey.Enter);

                Assert.Equal([0], lits);
                Assert.Contains("liquid", rows());
                Assert.DoesNotContain("to pour", rows());
            });
    }

    [Fact]
    public void LecternMentionFind_ManyAnswers_DownAndUpWrapAndEnterOpensTheSecondRow()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMenuSave(engine),
            static (send, rows, lit, _, console) =>
            {
                TLecternMention.TLecternMentionSeek(send, rows, console, TLecternMention.TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMention.TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                List<int> lits = [];
                TLecternMenuSend(console, lits, ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.UpArrow);

                send(ConsoleKey.Enter);

                Assert.Equal([0, 1, 0, 1], lits);
                Assert.Contains("to pour", rows());
                Assert.DoesNotContain("liquid", rows());
            });
    }

    private static void TLecternMenuSave(LEngine engine)
    {
        engine.TEngineEntrySave(TLecternMention.TLecternMentionCreate("water", [], "liquid"));
        engine.TEngineEntrySave(TLecternMention.TLecternMentionCreate("water", [], "to pour"));
        TLecternMention.TLecternMentionSave(engine, TLecternMention.TLecternMentionSentence, []);
    }

    private static int TLecternMenuRead(TConsoleFake console)
    {
        try
        {
            string[] grid = console.TConsoleFakeGrid;
            string?[][] roles = console.TConsoleFakeRoles;
            int[] menu = [.. Enumerable.Range(0, grid.Length).Where(row =>
                grid[row].Contains("water  English", StringComparison.Ordinal))];
            return menu.Length == 2
                ? Array.FindIndex(
                    menu, row => roles[row][grid[row].IndexOf("water", StringComparison.Ordinal)] == "lit")
                : -1;
        }
        catch (Exception exception) when (exception is InvalidOperationException or IndexOutOfRangeException)
        {
            return -1;
        }
    }

    private static void TLecternMenuSend(TConsoleFake console, List<int> lits, params ConsoleKey[] keys)
    {
        _ = Task.Run(() =>
        {
            Func<int> read = () => TLecternMenuRead(console);
            Assert.True(SpinWait.SpinUntil(() => read() >= 0, TimeSpan.FromSeconds(30)));
            lits.Add(read());
            foreach (ConsoleKey key in keys)
            {
                int before = read();
                console.TConsoleFakeKeys.Add(TLectern.TLecternKeyCreate(key));
                Assert.True(SpinWait.SpinUntil(() => read() != before, TimeSpan.FromSeconds(10)));
                lits.Add(read());
            }

            console.TConsoleFakeKeys.Add(TLectern.TLecternKeyCreate(ConsoleKey.Enter));
        });
    }

    private static void TLecternMenuSettle(TConsoleFake console, List<string> menu, params ConsoleKey[] keys)
    {
        _ = Task.Run(() =>
        {
            string[] grid = [];
            Func<bool> shown = () =>
            {
                try
                {
                    grid = [.. console.TConsoleFakeGrid];
                    return grid.Any(static row => row.Contains("Which entry?", StringComparison.Ordinal));
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            };
            if (SpinWait.SpinUntil(shown, TimeSpan.FromSeconds(30)))
            {
                menu.AddRange(grid);
            }

            foreach (ConsoleKey key in keys)
            {
                console.TConsoleFakeKeys.Add(TLectern.TLecternKeyCreate(key));
            }
        });
    }
}
