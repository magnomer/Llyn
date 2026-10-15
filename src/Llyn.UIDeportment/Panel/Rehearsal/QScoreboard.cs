using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QScoreboard
{
    private readonly UserControl _qScoreboardSurface;

    internal QScoreboard(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qScoreboardSurface = surface;
    }

    private TextBlock QScoreboardEmpty => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PScoreboardEmpty");

    internal void QScoreboardIntroduce()
    {
        CScoreboard scoreboard = new();
        QScoreboardEmpty.SetResourceReference(TextBlock.TextProperty, scoreboard.CScoreboardEmptyKey);
    }
}
