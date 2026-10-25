using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternWord
{
    [Fact]
    public void LecternWordStart_EnterOnExample_DrawsTheCursorOnTheFirstCharacter()
    {
        TLecternWordRun((line, route, noticed) =>
        {
            Assert.Equal(string.Empty, TLecternWordSend(line, Key.Right));

            Assert.Equal("t", TLecternWordSend(line, Key.Enter));
            Assert.Equal((0, 1), (route.CDisplayWordUnit, route.CDisplayWordLength));
            Assert.Equal("the water", string.Concat(line.Inlines.OfType<Run>().Select(static run => run.Text)));
            Assert.Empty(noticed);
        });
    }

    [Fact]
    public void LecternWordFind_EnterAfterMoving_LooksUpTheWordUnderTheCursor()
    {
        TLecternWordRun((line, route, noticed) =>
        {
            TLecternWordSend(line, Key.Enter);
            for (int step = 0; step < 5; step++)
            {
                TLecternWordSend(line, Key.Right);
            }

            Assert.Equal("a", TLecternWordSend(line, Key.Enter));
            (CMentionOffer? offer, int? unit) = Assert.Single(noticed);
            Assert.Equal(2, offer?.CMentionOfferEntry.Count);
            Assert.Equal(4, unit);
            Assert.Equal("w", route.CDisplayWordLead);
        });
    }

    [Fact]
    public void LecternWordStop_Escape_ClearsTheCursorAndEnterStartsAgain()
    {
        TLecternWordRun((line, route, noticed) =>
        {
            TLecternWordSend(line, Key.Enter);
            Assert.Equal("h", TLecternWordSend(line, Key.Right));

            Assert.Equal(string.Empty, TLecternWordSend(line, Key.Escape));
            Assert.Equal(string.Empty, TLecternWordSend(line, Key.Right));
            Assert.Equal(1, route.CDisplayWordUnit);

            Assert.Equal("t", TLecternWordSend(line, Key.Enter));
            Assert.Empty(noticed);
        });
    }

    private static void TLecternWordRun(Action<QMention, CDisplayRoute, List<(CMentionOffer?, int?)>> check)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        foreach (string meaning in new[] { "liquid", "to pour" })
        {
            engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate(meaning, 1)], []));
        }

        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate("the water");
        LEntry about = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "about",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1) with { LCardDraftSentence = [sentence] }],
            []));
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(about.LEntryId);
        CLeafLine example = Assert.Single(
            Assert.Single(wing.CWingDisplay.CDisplayCard.CDisplayCardRead().CLecternCardMeanings).CLeafSentence);

        TCardHinge.TCardHingeRun(() =>
        {
            QMention line = new();
            ItemsControl meanings = new() { Name = "PDisplayMeaning" };
            meanings.Items.Add(line);
            StackPanel surface = new();
            surface.Children.Add(meanings);
            surface.Children.Add(new StackPanel { Name = "PDisplayMeaningSection" });
            surface.Children.Add(new ItemsControl { Name = "PDisplayCollocation" });
            surface.Children.Add(new StackPanel { Name = "PDisplayCollocationSection" });
            List<(CMentionOffer?, int?)> noticed = [];
            TInterfaceDeportment.TLecternMentionAttach(
                TInterfaceDeportment.TLecternCardCreate(wing.CWingDisplay, surface),
                line,
                example.CLeafLineSentence,
                [.. example.CLeafLinePiece.Select(static piece => piece.CMentionPieceText)],
                (offer, unit) => noticed.Add((offer, unit)));
            using HwndSource source = new(new HwndSourceParameters("TLecternWord")) { RootVisual = surface };

            check(line, wing.CWingDisplay.CDisplayRoute, noticed);
        });
    }

    private static string TLecternWordSend(QMention line, Key key)
    {
        KeyEventArgs press = new(Keyboard.PrimaryDevice, PresentationSource.FromVisual(line), 0, key)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
        };
        line.RaiseEvent(press);
        press.RoutedEvent = Keyboard.KeyDownEvent;
        line.RaiseEvent(press);
        return string.Concat(line.Inlines.OfType<Run>()
            .Where(static run => run.ReadLocalValue(TextElement.BackgroundProperty) != DependencyProperty.UnsetValue)
            .Select(static run => run.Text));
    }
}
