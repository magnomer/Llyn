using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Llyn.UIDeportment;

internal static class PLeaf
{
    internal static void PLeafCardRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QLeafItem card)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardRank") is TextBlock rank)
        {
            rank.Text = card.QLeafItemRank;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardTitle") is TextBlock heading)
        {
            heading.Text = card.QLeafItemTitle;
            heading.Visibility = QLook.QLookVisibleRead(card.QLeafItemTitled);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardCaption") is TextBlock caption)
        {
            caption.Visibility = QLook.QLookVisibleRead(!card.QLeafItemTitled);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardCollocation") is TextBlock expression)
        {
            expression.Text = card.QLeafItemExpression;
            expression.Visibility = QLook.QLookVisibleRead(card.QLeafItemExpressed);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardMeaning") is TextBlock meaning)
        {
            meaning.Text = card.QLeafItemMeaning;
            meaning.Visibility = QLook.QLookVisibleRead(card.QLeafItemDefined);
        }

        if (QLook.QLookPartFind<ContentControl>(container, "PCardContents") is ContentControl contents)
        {
            contents.Content = card;
            PLeafBodyRefine(contents, card);
        }
    }

    private static void PLeafBodyRefine(FrameworkElement body, QLeafItem card)
    {
        PLeafListRefine(body, "PCardSituation", card.QLeafItemSituation, PLeafSituationRefine);
        PLeafListRefine(body, "PCardRegister", card.QLeafItemRegister, PLeafRegisterRefine);
        PLeafListRefine(body, "PCardTag", card.QLeafItemTag, PLeafTagRefine);
        if (QLook.QLookPartFind<ItemsControl>(body, "PCardTranslation") is ItemsControl translation)
        {
            translation.ItemsSource = card.QLeafItemTranslation;
            translation.Visibility = QLook.QLookVisibleRead(card.QLeafItemTranslation.Count > 0);
            if (card.QLeafItemSituation.Count == 0)
            {
                translation.SetResourceReference(FrameworkElement.MarginProperty, "Theme.Card.ChipMargin");
            }
            else
            {
                translation.ClearValue(FrameworkElement.MarginProperty);
            }

            QLookItem.QLookItemAttach(translation, PLeafLinkRefine);
        }

        if (QLook.QLookPartFind<Border>(body, "PCardExample") is Border example)
        {
            example.Visibility = QLook.QLookVisibleRead(card.QLeafItemSentence.Count > 0);
        }

        if (QLook.QLookPartFind<ItemsControl>(body, "PCardSentence") is ItemsControl sentences)
        {
            sentences.ItemsSource = card.QLeafItemSentence;
            QLookItem.QLookItemAttach(sentences, PLeafSentenceRefine);
        }

        if (QLook.QLookPartFind<ItemsControl>(body, "PCardImage") is ItemsControl images)
        {
            images.ItemsSource = card.QLeafItemImage;
            QLookItem.QLookItemAttach(images, QImageItem.QImageItemApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(body, "PCardVideo") is ItemsControl videos)
        {
            videos.ItemsSource = card.QLeafItemVideo;
            QLookItem.QLookItemAttach(videos, QVideoItem.QVideoItemApply);
        }
    }

    private static void PLeafListRefine<PLeafRow>(
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

    private static void PLeafSentenceRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QLeafLine line)
        {
            return;
        }

        TextBlock? frame = QLook.QLookPartFind<TextBlock>(container, "PExampleFrame");
        if (frame is not null)
        {
            frame.Text = line.QLeafLineHead;
        }

        if (QLook.QLookPartFind<PMention>(container, "PExampleText") is not PMention text)
        {
            return;
        }

        text.PMentionSentence = line.QLeafLineSentence;
        text.PMentionPiece = line.QLeafLinePiece;
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
            citation.Text = line.QLeafLineCitation;
            citation.SetBinding(
                FrameworkElement.MarginProperty, new Binding(nameof(PMention.Margin)) { Source = text });
            citation.SetBinding(
                TextBlock.FontFamilyProperty, new Binding(nameof(PMention.FontFamily)) { Source = text });
            citation.SetBinding(
                TextBlock.FontSizeProperty, new Binding(nameof(PMention.FontSize)) { Source = text });
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PExampleGloss") is ItemsControl gloss)
        {
            gloss.ItemsSource = line.QLeafLineGloss;
            QLookItem.QLookItemAttach(gloss, PGloss.PGlossRowApply);
        }
    }

    private static void PLeafSituationRefine(FrameworkElement container, object item, string? _)
    {
        if (item is QLeafChip situation
            && QLook.QLookPartFind<TextBlock>(container, "PSituationTitle") is TextBlock title)
        {
            title.Text = situation.QLeafChipText;
        }
    }

    private static void PLeafRegisterRefine(FrameworkElement container, object item, string? _)
    {
        if (item is QLeafChip register
            && QLook.QLookPartFind<TextBlock>(container, "PRegisterLabel") is TextBlock label)
        {
            label.Text = register.QLeafChipText;
        }
    }

    private static void PLeafTagRefine(FrameworkElement container, object item, string? _)
    {
        if (item is QLeafChip tag && QLook.QLookPartFind<TextBlock>(container, "PTagLabel") is TextBlock label)
        {
            label.Text = tag.QLeafChipText;
        }
    }

    private static void PLeafLinkRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QLinkChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PLinkFlag") is Image flag)
        {
            flag.Source = chip.QLinkChipFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkHeadword") is TextBlock headword)
        {
            headword.Text = chip.QLinkChipHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkLanguage") is TextBlock language)
        {
            language.Text = chip.QLinkChipLanguage;
        }
    }
}
