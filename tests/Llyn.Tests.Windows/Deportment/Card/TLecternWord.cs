using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternWord
{
    [Fact]
    public void LecternWordStart_EnterOnExample_DrawsCursorAtStart()
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
    public void LecternWordObserve_EnterAfterMoving_LooksUpCursorWord()
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
    public void LecternWordStop_Escape_ClearsCursorAndEnterStartsAgain()
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

    [Fact]
    public void LeafCardRefine_UnstoredChip_DisablesAndKeepsStoredOne()
    {
        TCardHinge.TCardHingeRun(() =>
        {
            CLeaf leaf = new(
                7, false, 1, TCardHinge.TCardHingeBlank, TCardHinge.TCardHingeMeaning,
                [new CLeafChip(3, new CStateWording("at home", null, false, null), CSubject.CSubjectSituation, true)],
                [],
                [new CLeafChip(0, new CStateWording("nature", null, false, null), CSubject.CSubjectTag, false)],
                [], [], [], [], false, true, [], [], TCardHinge.TCardHingeBlank, [], string.Empty, string.Empty,
                string.Empty);
            FrameworkElementFactory inner = new(typeof(StackPanel));
            foreach ((string part, string label) in
                     new[] { ("PCardSituation", "PSituationTitle"), ("PCardTag", "PTagLabel") })
            {
                FrameworkElementFactory chips = new(typeof(ItemsControl)) { Name = part };
                chips.SetValue(Control.TemplateProperty, new ControlTemplate(typeof(ItemsControl))
                {
                    VisualTree = new FrameworkElementFactory(typeof(ItemsPresenter)),
                });
                chips.SetValue(ItemsControl.ItemTemplateProperty, new DataTemplate
                {
                    VisualTree = new FrameworkElementFactory(typeof(TextBlock)) { Name = label },
                });
                inner.AppendChild(chips);
            }

            FrameworkElementFactory contents = new(typeof(ContentControl)) { Name = "PCardContents" };
            contents.SetValue(Control.TemplateProperty, new ControlTemplate(typeof(ContentControl))
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentPresenter)),
            });
            contents.SetValue(ContentControl.ContentTemplateProperty, new DataTemplate { VisualTree = inner });
            FrameworkElementFactory card = new(typeof(StackPanel));
            card.AppendChild(contents);
            ItemsControl list = TCardHinge.TCardHingeLoad(card);
            list.ItemsSource = new[] { TInterfaceDeportment.TLeafItemCreate(leaf, leaf.CLeafMeaning) };
            TCardHinge.TCardHingeSettle(list);
            TInterfaceDeportment.TLeafCardRefine(
                (ContentPresenter)list.ItemContainerGenerator.ContainerFromIndex(0), list.Items[0]);
            TCardHinge.TCardHingeSettle(list);
            Dictionary<string, bool> enabled = [];
            Queue<DependencyObject> pending = new([list]);
            while (pending.TryDequeue(out DependencyObject? node))
            {
                if (node is TextBlock { Text: "at home" or "nature" } chip)
                {
                    enabled[chip.Text] = chip.IsEnabled;
                }

                for (int index = 0; index < VisualTreeHelper.GetChildrenCount(node); index++)
                {
                    pending.Enqueue(VisualTreeHelper.GetChild(node, index));
                }
            }

            Assert.Equal(new Dictionary<string, bool> { ["at home"] = true, ["nature"] = false }, enabled);
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
