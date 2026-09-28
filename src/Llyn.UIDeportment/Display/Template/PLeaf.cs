using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PLeaf
{
    internal PLinkConverter PLeafLink { get; } = new();

    internal QCitationConverter PLeafCitation { get; } = new();

    internal PSentenceConverter PLeafFrame { get; } = new();

    internal void PLeafCardApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LCardDraft card)
        {
            return;
        }

        QStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");
        string title = (string)state.Convert(
            [CFolio.CFolioStateRead(card.LCardDraftTitle), unknown], typeof(string), string.Empty, culture);
        if (QLook.QLookPartFind<TextBlock>(container, "PCardRank") is TextBlock rank)
        {
            rank.Text = card.LCardDraftPosition.ToString(culture);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardTitle") is TextBlock heading)
        {
            heading.Text = title;
            heading.Visibility = QLook.QLookVisibleRead(title.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardCaption") is TextBlock caption)
        {
            caption.Visibility = QLook.QLookVisibleRead(title.Length == 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardCollocation") is TextBlock expression)
        {
            string text = (string)state.Convert(
                [CFolio.CFolioStateRead(card.LCardDraftExpression), unknown], typeof(string), string.Empty, culture);
            expression.Text = text;
            expression.Visibility = QLook.QLookVisibleRead(text.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardMeaning") is TextBlock meaning)
        {
            string text = (string)state.Convert(
                [CFolio.CFolioStateRead(card.LCardDraftMeaning), unknown], typeof(string), string.Empty, culture);
            meaning.Text = text;
            meaning.Visibility = QLook.QLookVisibleRead(text.Length > 0);
        }

        if (QLook.QLookPartFind<ContentControl>(container, "PCardContents") is ContentControl contents)
        {
            contents.Content = card;
            PLeafBodyApply(contents, card, culture);
        }
    }

    private void PLeafBodyApply(FrameworkElement body, LCardDraft card, CultureInfo culture)
    {
        PLeafListApply(body, "PCardSituation", card.LCardDraftSituation, PLeafSituationApply);
        PLeafListApply(body, "PCardRegister", card.LCardDraftRegister, PLeafRegisterApply);
        PLeafListApply(body, "PCardTag", card.LCardDraftTag, PLeafTagApply);
        if (QLook.QLookPartFind<ItemsControl>(body, "PCardTranslation") is ItemsControl translation)
        {
            translation.ItemsSource = (IEnumerable<LLinkChip>)PLeafLink.Convert(
                card.LCardDraftTranslation, typeof(IEnumerable<LLinkChip>), string.Empty, culture);
            translation.Visibility = QLook.QLookVisibleRead(card.LCardDraftTranslation.Count > 0);
            if (card.LCardDraftSituation.Count == 0)
            {
                translation.SetResourceReference(FrameworkElement.MarginProperty, "Theme.Card.ChipMargin");
            }
            else
            {
                translation.ClearValue(FrameworkElement.MarginProperty);
            }

            QLookItem.QLookItemAttach(translation, PLeafLinkApply);
        }

        if (QLook.QLookPartFind<Border>(body, "PCardExample") is Border example)
        {
            example.Visibility = QLook.QLookVisibleRead(card.LCardDraftSentence.Count > 0);
        }

        if (QLook.QLookPartFind<ItemsControl>(body, "PCardSentence") is ItemsControl sentences)
        {
            sentences.ItemsSource = card.LCardDraftSentence;
            QLookItem.QLookItemAttach(sentences, PLeafSentenceApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(body, "PCardImage") is ItemsControl images)
        {
            images.ItemsSource = card.LCardDraftImage;
            QLookItem.QLookItemAttach(images, PImage.PImageLineApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(body, "PCardVideo") is ItemsControl videos)
        {
            videos.ItemsSource = card.LCardDraftVideo;
            QLookItem.QLookItemAttach(videos, PVideo.PVideoLineApply);
        }
    }

    private static void PLeafListApply<PLeafRow>(
        FrameworkElement body,
        string name,
        IReadOnlyList<PLeafRow> rows,
        Action<FrameworkElement, object, string?> fill)
    {
        if (QLook.QLookPartFind<ItemsControl>(body, name) is not ItemsControl list)
        {
            return;
        }

        list.ItemsSource = rows;
        list.Visibility = QLook.QLookVisibleRead(rows.Count > 0);
        QLookItem.QLookItemAttach(list, fill);
    }

    private void PLeafSentenceApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LSentenceDraft sentence)
        {
            return;
        }

        CultureInfo culture = CultureInfo.CurrentCulture;
        LExampleDraft? example = sentence.LSentenceDraftExample;
        object[] parts =
        [
            CFolio.CFolioStateRead(sentence.LSentenceDraftParticle),
            CFolio.CFolioStateRead(sentence.LSentenceDraftDependence),
            CFolio.CFolioStateRead(
                sentence.LSentenceDraftExample?.LExampleDraftText ?? LStateValue.LStateValueUnspecified),
            QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
        ];
        TextBlock? frame = QLook.QLookPartFind<TextBlock>(container, "PExampleFrame");
        if (frame is not null)
        {
            frame.Text = (string)PLeafFrame.Convert(parts, typeof(string), "Head", culture);
        }

        if (QLook.QLookPartFind<PMention>(container, "PExampleText") is not PMention text)
        {
            return;
        }

        text.PMentionLanguage = example?.LExampleDraftLanguage ?? string.Empty;
        text.PMentionMention = CMention.CMentionMarkRead(example?.LExampleDraftMention);
        text.PMentionText = (string)PLeafFrame.Convert(parts, typeof(string), "Text", culture);
        if (frame is not null)
        {
            MultiBinding margin = new() { Converter = new QFontConverter() };
            margin.Bindings.Add(new Binding(nameof(TextBlock.FontFamily)) { Source = frame });
            margin.Bindings.Add(new Binding(nameof(TextBlock.FontSize)) { Source = frame });
            margin.Bindings.Add(new Binding(nameof(PMention.FontFamily)) { Source = text });
            margin.Bindings.Add(new Binding(nameof(PMention.FontSize)) { Source = text });
            text.SetBinding(FrameworkElement.MarginProperty, margin);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PExampleCitation") is TextBlock citation)
        {
            citation.Text = (string)PLeafCitation.Convert(
                example?.LExampleDraftReference.LStateAnchorShown ?? (object)string.Empty,
                typeof(string),
                string.Empty,
                culture);
            citation.SetBinding(
                FrameworkElement.MarginProperty, new Binding(nameof(PMention.Margin)) { Source = text });
            citation.SetBinding(
                TextBlock.FontFamilyProperty, new Binding(nameof(PMention.FontFamily)) { Source = text });
            citation.SetBinding(
                TextBlock.FontSizeProperty, new Binding(nameof(PMention.FontSize)) { Source = text });
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PExampleGloss") is ItemsControl gloss)
        {
            gloss.ItemsSource = (IEnumerable<PGloss>)new PGlossConverter().Convert(
                CFolio.CFolioGlossRead(sentence.LSentenceDraftExample?.LExampleDraftGloss ?? []),
                typeof(IEnumerable<PGloss>),
                string.Empty,
                culture);
            QLookItem.QLookItemAttach(gloss, PGloss.PGlossRowApply);
        }
    }

    private static void PLeafSituationApply(FrameworkElement container, object item, string? _)
    {
        if (item is LSituationDraft situation
            && QLook.QLookPartFind<TextBlock>(container, "PSituationTitle") is TextBlock title)
        {
            title.Text = PLeafStateRead(situation.LSituationDraftTitle);
        }
    }

    private static void PLeafRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is LRegisterDraft register
            && QLook.QLookPartFind<TextBlock>(container, "PRegisterLabel") is TextBlock label)
        {
            label.Text = PLeafStateRead(register.LRegisterDraftName);
        }
    }

    private static void PLeafTagApply(FrameworkElement container, object item, string? _)
    {
        if (item is LTagDraft tag && QLook.QLookPartFind<TextBlock>(container, "PTagLabel") is TextBlock label)
        {
            label.Text = tag.LTagDraftText;
        }
    }

    private static void PLeafLinkApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LLinkChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PLinkFlag") is Image flag)
        {
            flag.Source = chip.LLinkChipFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkHeadword") is TextBlock headword)
        {
            headword.Text = chip.LLinkChipHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkLanguage") is TextBlock language)
        {
            language.Text = chip.LLinkChipLanguage;
        }
    }

    private static string PLeafStateRead(LStateValue value)
    {
        return (string)new QStateConverter().Convert(
            [CFolio.CFolioStateRead(value), QLocalizationCatalog.QLocalizationTextRead("Display.Unknown")],
            typeof(string),
            string.Empty,
            CultureInfo.CurrentCulture);
    }
}
