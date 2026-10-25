using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternMention
{
    internal const string TLecternMentionSentence = "The water is cold.";

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
    public void LecternMentionFind_StoredSenseFarBelow_ScrollsToItsCardAndLightsTheHeader()
    {
        TLecternCard.TLecternCardRun(
            static (engine, save) =>
            {
                long water = save(
                    "water",
                    [
                        .. Enumerable.Range(1, 12).Select(static index =>
                            TInterface.TCardCreate("definition " + index, index) with
                            {
                                LCardDraftTitle = "sense" + index,
                            }),
                    ]);
                long sense = engine.TEngineEntryLoad(water)!.LEntryDraftMeanings
                    .Single(static card => card.LCardDraftPosition == 6).LCardDraftId;
                TLecternMentionSave(
                    engine,
                    TLecternMentionSentence,
                    [TInterfaceMentionSpan.TMentionDraftCreate(0, 4, 5, water, sense)]);
            },
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                TLecternMentionSend(console, lit, ConsoleKey.RightArrow);

                send(ConsoleKey.Enter);

                Assert.EndsWith("sense6", rows()[0], StringComparison.Ordinal);
                Assert.Equal(("▾ sense6", "lit"), lit());
            },
            height: 24);
    }

    [Fact]
    public void LecternMentionUpDown_LeavingTheSentenceAndComingBack_DoesNotReturnTheCursor()
    {
        const string other = "The sky is blue.";
        TLecternCard.TLecternCardRun(
            static (engine, _) => engine.TEngineEntrySave(TLecternMentionCreate(
                "about", [TLecternSentenceCreate(TLecternMentionSentence, []), TLecternSentenceCreate(other, [])])),
            static (send, rows, lit, _, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                Assert.True(TLecternMentionCheck(console));

                send(ConsoleKey.DownArrow);
                Assert.Equal((other, "lit"), lit());
                send(ConsoleKey.UpArrow);

                Assert.Equal((TLecternMentionSentence, "lit"), lit());
                Assert.False(TLecternMentionCheck(console));
            });
    }

    [Fact]
    public void LecternMentionRefresh_OtherEntryWithTheSameItemName_DropsTheCursor()
    {
        long other = 0;
        TLecternCard.TLecternCardRun(
            (engine, _) =>
            {
                TLecternMentionSave(engine, TLecternMentionSentence, []);
                other = TLecternMentionSave(engine, "Zebras run fast.", [], "zebra");
            },
            (send, rows, lit, atelier, console) =>
            {
                TLecternMentionSeek(send, rows, console, TLecternMentionSentence);
                send(ConsoleKey.Enter);
                send(ConsoleKey.RightArrow);
                Assert.True(TLecternMentionCheck(console));

                atelier.CAtelierNavigation.CNavigationEntryOpen(other);

                Assert.Contains(rows(), static row => row.Contains("Zebras run fast.", StringComparison.Ordinal));
                Assert.False(TLecternMentionCheck(console));
                Assert.NotEqual("Z", lit()?.Item1);
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

    internal static LEntryDraft TLecternMentionCreate(
        string headword, IReadOnlyList<LSentenceDraft> sentences, string meaning = "a thing") =>
        TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate(meaning, 1) with { LCardDraftSentence = sentences }],
            []);

    internal static long TLecternMentionSave(
        LEngine engine, string text, IReadOnlyList<LMentionDraft> mentions, string headword = "about") =>
        engine.TEngineEntrySave(TLecternMentionCreate(headword, [TLecternSentenceCreate(text, mentions)])).LEntryId;

    internal static void TLecternMentionSeek(
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

    internal static bool TLecternMentionCheck(TConsoleFake console) =>
        console.TConsoleFakeGrid.Any(static row => row.Contains(TLecternMentionFoot, StringComparison.Ordinal));

    internal static void TLecternMentionSend(TConsoleFake console, Func<(string, string)?> lit, ConsoleKey key)
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

    private static LSentenceDraft TLecternSentenceCreate(string text, IReadOnlyList<LMentionDraft> mentions) =>
        TInterfaceExample.TSentenceDraftCreate(text) with
        {
            LSentenceDraftExample =
                TInterfaceExample.TExampleDraftCreate(text) with { LExampleDraftMention = mentions },
        };
}
