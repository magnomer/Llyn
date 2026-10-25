using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternMention
{
    private const string TLecternMentionSentence = "The water is cold.";

    private const string TLecternMentionFoot = "Word: ←→ move  Enter look up  Esc stop";

    [Fact]
    public void LecternMentionStart_EnterOnExample_LightsTheFirstCharacterAndShowsTheWordFoot()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMentionSave(engine, TLecternMentionSentence, []),
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                Assert.Equal((TLecternMentionSentence, "lit"), lit());
                Assert.False(TLecternMentionCheck(console));

                send(ConsoleKey.Enter);

                Assert.Equal(("T", "lit"), lit());
                Assert.True(TLecternMentionCheck(console));
                Assert.Contains(rows(), static row => row.EndsWith(TLecternMentionSentence, StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternMentionMove_ArrowsAndControlArrows_StepCharactersAndJumpToWordsAndPieces()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
            {
                long water = engine.TEngineEntrySave(TLecternMentionCreate("water", [])).LEntryId;
                TLecternMentionSave(
                    engine, TLecternMentionSentence, [TInterfaceMentionSpan.TMentionDraftCreate(0, 4, 3, water)]);
            },
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);

                send(ConsoleKey.RightArrow);
                Assert.Equal(("h", "lit"), lit());
                send(ConsoleKey.LeftArrow);
                send(ConsoleKey.LeftArrow);
                Assert.Equal(("T", "lit"), lit());

                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                Assert.Equal(("w", "lit"), lit());
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                Assert.Equal(("e", "lit"), lit());
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                Assert.Equal(("i", "lit"), lit());
                TLecternMentionSend(console, lit, ConsoleKey.LeftArrow);
                Assert.Equal(("e", "lit"), lit());
            });
    }

    [Fact]
    public void LecternMentionMove_CjkWithoutSpaces_StepsOneCharacter()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMentionSave(engine, "水很冷。", []),
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, "水很冷。");
                send(ConsoleKey.Enter);

                send(ConsoleKey.RightArrow);

                Assert.Equal(("很", "lit"), lit());
            });
    }

    [Fact]
    public void LecternMentionFind_SoleAnswer_OpensTheEntry()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
            {
                engine.TEngineEntrySave(TLecternMentionCreate("water", []));
                TLecternMentionSave(engine, TLecternMentionSentence, []);
            },
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);

                send(ConsoleKey.Enter);

                Assert.StartsWith("water", rows()[0], StringComparison.Ordinal);
                Assert.DoesNotContain(console.TConsoleFakeShots, static shot => shot.Any(static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal)));
                Assert.False(TLecternMentionCheck(console));
            });
    }

    [Fact]
    public void LecternMentionFind_StoredMention_OpensItsEntryWithoutAMenu()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
            {
                long first = engine.TEngineEntrySave(TLecternMentionCreate("water", [], "liquid")).LEntryId;
                engine.TEngineEntrySave(TLecternMentionCreate("water", [], "to pour"));
                TLecternMentionSave(
                    engine, TLecternMentionSentence, [TInterfaceMentionSpan.TMentionDraftCreate(0, 4, 5, first)]);
            },
            static (send, rows, lit, _, console) =>
            {
                Assert.Equal("link", TLectern.TLecternRoleRead(console, "water"));
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);

                send(ConsoleKey.Enter);

                Assert.StartsWith("water", rows()[0], StringComparison.Ordinal);
                Assert.Contains("liquid", rows());
            });
    }

    [Fact]
    public void LecternMentionFind_ManyAnswers_OpensTheMenuUnderTheWordAndOpensThePick()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
            {
                engine.TEngineEntrySave(TLecternMentionCreate("water", [], "liquid"));
                engine.TEngineEntrySave(TLecternMentionCreate("water", [], "to pour"));
                TLecternMentionSave(engine, TLecternMentionSentence, []);
            },
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                send(ConsoleKey.RightArrow);
                List<string> menu = [];
                TLecternMentionSettle(console, menu, ConsoleKey.DownArrow, ConsoleKey.Enter);

                send(ConsoleKey.Enter);

                int line = menu.FindIndex(static row =>
                    row.Contains(TLecternMentionSentence, StringComparison.Ordinal));
                Assert.True(line >= 0);
                int word = menu[line].IndexOf("water", StringComparison.Ordinal);
                Assert.Equal('╔', menu[line + 1][word]);
                Assert.Contains("Which entry?", menu[line + 1], StringComparison.Ordinal);
                Assert.Equal(2, menu.Count(static row => row.Contains("water  English", StringComparison.Ordinal)));
                Assert.StartsWith("water", rows()[0], StringComparison.Ordinal);
                Assert.Contains(rows(), static row => row is "liquid" or "to pour");
                Assert.DoesNotContain(console.TConsoleFakeGrid, static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal));
            });
    }

    [Fact]
    public void LecternMentionFind_MenuEscape_ClosesTheMenuAndKeepsTheCursor()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
            {
                engine.TEngineEntrySave(TLecternMentionCreate("water", [], "liquid"));
                engine.TEngineEntrySave(TLecternMentionCreate("water", [], "to pour"));
                TLecternMentionSave(engine, TLecternMentionSentence, []);
            },
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                List<string> menu = [];
                TLecternMentionSettle(console, menu, ConsoleKey.Escape);

                send(ConsoleKey.Enter);

                Assert.NotEmpty(menu);
                Assert.Equal(("w", "lit"), lit());
                Assert.Contains(rows(), static row => row.EndsWith(TLecternMentionSentence, StringComparison.Ordinal));
                Assert.DoesNotContain(console.TConsoleFakeGrid, static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal));
                Assert.True(TLecternMentionCheck(console));
            });
    }

    [Fact]
    public void LecternMentionFind_Landing_ClosesTheMenu()
    {
        long landing = 0;
        TLecternCard.TLecternCardRun(
            (engine, _) =>
            {
                landing = engine.TEngineEntrySave(TLecternMentionCreate("water", [], "liquid")).LEntryId;
                engine.TEngineEntrySave(TLecternMentionCreate("water", [], "to pour"));
                TLecternMentionSave(engine, TLecternMentionSentence, []);
            },
            (send, rows, lit, atelier, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                SynchronizationContext loop = SynchronizationContext.Current!;
                List<string> menu = [];
                TLecternMentionSettle(console, menu);
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
            static (engine, _) => TLecternMentionSave(engine, TLecternMentionSentence, []),
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);

                send(ConsoleKey.Enter);

                Assert.Equal(("T", "lit"), lit());
                Assert.DoesNotContain(console.TConsoleFakeShots, static shot => shot.Any(static row =>
                    row.Contains("Which entry?", StringComparison.Ordinal)));
            });
    }

    [Fact]
    public void LecternMentionStop_Escape_LeavesTheCursorAndKeepsTheLineLit()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternMentionSave(engine, TLecternMentionSentence, []),
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                send(ConsoleKey.RightArrow);

                send(ConsoleKey.Escape);

                Assert.Equal((TLecternMentionSentence, "lit"), lit());
                Assert.False(TLecternMentionCheck(console));

                send(ConsoleKey.Enter);

                Assert.Equal(("T", "lit"), lit());
            });
    }

    [Fact]
    public void LecternMentionMove_WrappedSentence_FollowsTheCursorOntoTheNextRowAndOpensTheWord()
    {
        const string text =
            "The cold water ran over the stones and the light was clear and the brook was wide and deep by the quarry";
        TLecternCard.TLecternCardRun(
            static (engine, _) =>
            {
                engine.TEngineEntrySave(TLecternMentionCreate("quarry", []));
                TLecternMentionSave(engine, text, []);
            },
            static (send, rows, lit, _, console) =>
            {
                Assert.DoesNotContain(rows(), static row => row.EndsWith(text, StringComparison.Ordinal));
                TLecternMentionSeek(send, rows, console, "The cold water");
                send(ConsoleKey.Enter);

                for (int step = 0; step < 30 && lit()?.Item1 != "q"; step++)
                {
                    TLecternMentionSend(console, lit, ConsoleKey.RightArrow);
                }

                List<string> shown = [.. rows()];
                Assert.Equal(("q", "lit"), lit());
                Assert.True(shown.FindIndex(static row => row.Contains("quarry", StringComparison.Ordinal))
                    > shown.FindIndex(static row => row.Contains("The cold water", StringComparison.Ordinal)));

                send(ConsoleKey.Enter);

                Assert.StartsWith("quarry", rows()[0], StringComparison.Ordinal);
            });
    }

    private static LEntryDraft TLecternMentionCreate(
        string headword, IReadOnlyList<LSentenceDraft> sentences, string meaning = "a thing") =>
        TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate(meaning, 1) with { LCardDraftSentence = sentences }],
            []);

    private static void TLecternMentionSave(LEngine engine, string text, IReadOnlyList<LMentionDraft> mentions) =>
        engine.TEngineEntrySave(TLecternMentionCreate(
            "about",
            [
                TInterfaceExample.TSentenceDraftCreate(text) with
                {
                    LSentenceDraftExample =
                        TInterfaceExample.TExampleDraftCreate(text) with { LExampleDraftMention = mentions },
                },
            ]));

    private static void TLecternMentionSeek(
        Action<ConsoleKey> send, Func<IReadOnlyList<string>> rows, TConsoleFake console, string text)
    {
        for (int step = 0; step < 30; step++)
        {
            _ = rows();
            if (TLectern.TLecternRoleRead(console, text) == "lit")
            {
                return;
            }

            send(ConsoleKey.DownArrow);
        }

        Assert.Fail($"The example '{text}' was never lit.");
    }

    private static bool TLecternMentionCheck(TConsoleFake console) =>
        console.TConsoleFakeGrid.Any(static row => row.Contains(TLecternMentionFoot, StringComparison.Ordinal));

    private static void TLecternMentionSend(TConsoleFake console, Func<(string, string)?> lit, ConsoleKey key)
    {
        Func<string> roles = () =>
        {
            _ = lit();
            return string.Join('\n', console.TConsoleFakeRoles.Select(static row => string.Join(',', row)));
        };
        string before = roles();
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', key, false, false, true));
        Assert.True(SpinWait.SpinUntil(() => roles() != before, TimeSpan.FromSeconds(10)));
    }

    private static void TLecternMentionSettle(TConsoleFake console, List<string> menu, params ConsoleKey[] keys)
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
