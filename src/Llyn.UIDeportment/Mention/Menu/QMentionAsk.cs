using System;
using System.Windows;

namespace Llyn.UIDeportment;

internal sealed class QMentionAsk
{
    internal QMentionAsk(FrameworkElement anchor)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        QMentionAskAnchor = anchor;
    }

    internal event Action<FrameworkElement, long>? QMentionAskChosen;

    internal FrameworkElement QMentionAskAnchor { get; }

    internal void QMentionAskSettle(long sense)
    {
        QMentionAskChosen?.Invoke(QMentionAskAnchor, sense);
    }
}
