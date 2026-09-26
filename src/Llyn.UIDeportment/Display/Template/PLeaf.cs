using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PLeaf
{
    internal PLinkConverter PLeafLink { get; } = new();

    internal PCitationConverter PLeafCitation { get; } = new();

    internal PSentenceConverter PLeafFrame { get; } = new();

    internal void PLeafCardApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LCardDraft card)
        {
            return;
        }

        PStateConverter state = new();
        CultureInfo culture = CultureInfo.CurrentCulture;
        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string title = (string)state.Convert([card.LCardDraftTitle, unknown], typeof(string), string.Empty, culture);
        if (PLook.PLookPartFind<TextBlock>(container, "PCardRank") is TextBlock rank)
        {
            rank.Text = card.LCardDraftPosition.ToString(culture);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCardTitle") is TextBlock heading)
        {
            heading.Text = title;
            heading.Visibility = PLook.PLookVisibleRead(title.Length > 0);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCardCaption") is TextBlock caption)
        {
            caption.Visibility = PLook.PLookVisibleRead(title.Length == 0);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCardCollocation") is TextBlock expression)
        {
            string text = (string)state.Convert(
                [card.LCardDraftExpression, unknown], typeof(string), string.Empty, culture);
            expression.Text = text;
            expression.Visibility = PLook.PLookVisibleRead(text.Length > 0);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCardMeaning") is TextBlock meaning)
        {
            string text = (string)state.Convert(
                [card.LCardDraftMeaning, unknown], typeof(string), string.Empty, culture);
            meaning.Text = text;
            meaning.Visibility = PLook.PLookVisibleRead(text.Length > 0);
        }

        if (PLook.PLookPartFind<ContentControl>(container, "PCardContents") is ContentControl contents)
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
        if (PLook.PLookPartFind<ItemsControl>(body, "PCardTranslation") is ItemsControl translation)
        {
            translation.ItemsSource = (IEnumerable<LLinkChip>)PLeafLink.Convert(
                card.LCardDraftTranslation, typeof(IEnumerable<LLinkChip>), string.Empty, culture);
            translation.Visibility = PLook.PLookVisibleRead(card.LCardDraftTranslation.Count > 0);
            if (card.LCardDraftSituation.Count == 0)
            {
                translation.SetResourceReference(FrameworkElement.MarginProperty, "Theme.Card.ChipMargin");
            }
            else
            {
                translation.ClearValue(FrameworkElement.MarginProperty);
            }

            PLookItem.PLookItemAttach(translation, PLeafLinkApply);
        }

        if (PLook.PLookPartFind<Border>(body, "PCardExample") is Border example)
        {
            example.Visibility = PLook.PLookVisibleRead(card.LCardDraftSentence.Count > 0);
        }

        if (PLook.PLookPartFind<ItemsControl>(body, "PCardSentence") is ItemsControl sentences)
        {
            sentences.ItemsSource = card.LCardDraftSentence;
            PLookItem.PLookItemAttach(sentences, PLeafSentenceApply);
        }

        if (PLook.PLookPartFind<ItemsControl>(body, "PCardImage") is ItemsControl images)
        {
            images.ItemsSource = card.LCardDraftImage;
            PLookItem.PLookItemAttach(images, PImage.PImageLineApply);
        }

        if (PLook.PLookPartFind<ItemsControl>(body, "PCardVideo") is ItemsControl videos)
        {
            videos.ItemsSource = card.LCardDraftVideo;
            PLookItem.PLookItemAttach(videos, PVideo.PVideoLineApply);
        }
    }

    private static void PLeafListApply<PLeafRow>(
        FrameworkElement body,
        string name,
        IReadOnlyList<PLeafRow> rows,
        Action<FrameworkElement, object, string?> fill)
    {
        if (PLook.PLookPartFind<ItemsControl>(body, name) is not ItemsControl list)
        {
            return;
        }

        list.ItemsSource = rows;
        list.Visibility = PLook.PLookVisibleRead(rows.Count > 0);
        PLookItem.PLookItemAttach(list, fill);
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
            sentence.LSentenceDraftParticle,
            sentence.LSentenceDraftDependence,
            example?.LExampleDraftText ?? (object)string.Empty,
            PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
        ];
        TextBlock? frame = PLook.PLookPartFind<TextBlock>(container, "PExampleFrame");
        if (frame is not null)
        {
            frame.Text = (string)PLeafFrame.Convert(parts, typeof(string), "Head", culture);
        }

        if (PLook.PLookPartFind<PMention>(container, "PExampleText") is not PMention text)
        {
            return;
        }

        text.PMentionLanguage = example?.LExampleDraftLanguage ?? string.Empty;
        text.PMentionMention = (IReadOnlyList<LMention>)new PMentionConverter().Convert(
            example?.LExampleDraftMention, typeof(IReadOnlyList<LMention>), null, culture);
        text.PMentionText = (string)PLeafFrame.Convert(parts, typeof(string), "Text", culture);
        if (frame is not null)
        {
            MultiBinding margin = new() { Converter = new PFontConverter() };
            margin.Bindings.Add(new Binding(nameof(TextBlock.FontFamily)) { Source = frame });
            margin.Bindings.Add(new Binding(nameof(TextBlock.FontSize)) { Source = frame });
            margin.Bindings.Add(new Binding(nameof(PMention.FontFamily)) { Source = text });
            margin.Bindings.Add(new Binding(nameof(PMention.FontSize)) { Source = text });
            text.SetBinding(FrameworkElement.MarginProperty, margin);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PExampleCitation") is TextBlock citation)
        {
            citation.Text = (string)PLeafCitation.Convert(
                example?.LExampleDraftReference ?? (object)string.Empty, typeof(string), string.Empty, culture);
            citation.SetBinding(
                FrameworkElement.MarginProperty, new Binding(nameof(PMention.Margin)) { Source = text });
            citation.SetBinding(
                TextBlock.FontFamilyProperty, new Binding(nameof(PMention.FontFamily)) { Source = text });
            citation.SetBinding(
                TextBlock.FontSizeProperty, new Binding(nameof(PMention.FontSize)) { Source = text });
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PExampleGloss") is ItemsControl gloss)
        {
            gloss.ItemsSource = (IEnumerable<PGloss>)new PGlossConverter().Convert(
                example?.LExampleDraftGloss ?? (object)string.Empty,
                typeof(IEnumerable<PGloss>),
                string.Empty,
                culture);
            PLookItem.PLookItemAttach(gloss, PGloss.PGlossRowApply);
        }
    }

    private static void PLeafSituationApply(FrameworkElement container, object item, string? _)
    {
        if (item is LSituationDraft situation
            && PLook.PLookPartFind<TextBlock>(container, "PSituationTitle") is TextBlock title)
        {
            title.Text = PLeafStateRead(situation.LSituationDraftTitle);
        }
    }

    private static void PLeafRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is LRegisterDraft register
            && PLook.PLookPartFind<TextBlock>(container, "PRegisterLabel") is TextBlock label)
        {
            label.Text = PLeafStateRead(register.LRegisterDraftName);
        }
    }

    private static void PLeafTagApply(FrameworkElement container, object item, string? _)
    {
        if (item is LTagDraft tag && PLook.PLookPartFind<TextBlock>(container, "PTagLabel") is TextBlock label)
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

        if (PLook.PLookPartFind<Image>(container, "PLinkFlag") is Image flag)
        {
            flag.Source = chip.LLinkChipFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PLinkHeadword") is TextBlock headword)
        {
            headword.Text = chip.LLinkChipHeadword;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PLinkLanguage") is TextBlock language)
        {
            language.Text = chip.LLinkChipLanguage;
        }
    }

    private static string PLeafStateRead(LStateValue value)
    {
        return (string)new PStateConverter().Convert(
            [value, PLocalizationCatalog.PLocalizationTextRead("Display.Unknown")],
            typeof(string),
            string.Empty,
            CultureInfo.CurrentCulture);
    }
}
