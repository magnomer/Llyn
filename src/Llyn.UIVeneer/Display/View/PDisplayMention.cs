using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayMentionHandle(object? sender, PMentionArgument e)
    {
        if (e.OriginalSource is not PMention shown)
        {
            return;
        }

        string language = shown.PMentionLanguage.Length > 0 ? shown.PMentionLanguage : PDisplayLanguage.Text;

        LMentionResult result;
        try
        {
            result = _lDisplay.LDisplayMentionFind(
                shown.PMentionText, language, e.PMentionArgumentOffset, shown.PMentionMention ?? []);
        }
        catch (Exception exception)
        {
            _pDisplayHost.PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        _pDisplayHost.PWindowMentionHandle(shown, result);
    }

    internal void PDisplayCardScroll(long id)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            FrameworkElement? card = PDisplayCardFind(PDisplayMeaning, id) ?? PDisplayCardFind(PDisplayCollocation, id);
            if (card is null || PCompassOffsetRead(card) is not double top)
            {
                return;
            }

            PDisplayContents.ScrollToVerticalOffset(PDisplayContents.VerticalOffset + top - PCompassLead);
            ((Storyboard)FindResource("Theme.Card.Spotlight")).Begin(card);
        });
    }

    private static FrameworkElement? PDisplayCardFind(ItemsControl cards, long id)
    {
        for (int index = 0; index < cards.Items.Count; index++)
        {
            if (cards.Items[index] is LCardDraft card && card.LCardDraftId == id)
            {
                return cards.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
            }
        }

        return null;
    }
}
