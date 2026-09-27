using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QColophon
{
    private readonly UserControl _qColophonSurface;

    internal QColophon(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qColophonSurface = surface;
    }

    private StackPanel QColophonBody => QContract.QContractFind<StackPanel>(_qColophonSurface, "PColophonBody");

    private TextBlock QColophonTitle => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonTitle");

    private Border QColophonChip => QContract.QContractFind<Border>(_qColophonSurface, "PColophonChip");

    private TextBlock QColophonKind => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonKind");

    private TextBlock QColophonTally => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonTally");

    private StackPanel QColophonAuthorSection =>
        QContract.QContractFind<StackPanel>(_qColophonSurface, "PColophonAuthorSection");

    private TextBlock QColophonAuthor => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonAuthor");

    private StackPanel QColophonYearSection =>
        QContract.QContractFind<StackPanel>(_qColophonSurface, "PColophonYearSection");

    private TextBlock QColophonYear => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonYear");

    private StackPanel QColophonUrlSection =>
        QContract.QContractFind<StackPanel>(_qColophonSurface, "PColophonUrlSection");

    private TextBlock QColophonUrl => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonUrl");

    private StackPanel QColophonNoteSection =>
        QContract.QContractFind<StackPanel>(_qColophonSurface, "PColophonNoteSection");

    private TextBlock QColophonNote => QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonNote");

    private TextBlock QColophonUnselected =>
        QContract.QContractFind<TextBlock>(_qColophonSurface, "PColophonUnselected");

    internal void QColophonShow(CColophon sheet)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        QColophonTitle.Text = sheet.CColophonTitle;
        QField.QFieldPlaceholderShow(QColophonTitle, sheet.CColophonTitleFaint);
        QColophonKind.Text = sheet.CColophonKind;
        QColophonChip.Visibility = QLook.QLookVisibleRead(sheet.CColophonKindShown);
        QColophonYear.Text = sheet.CColophonYear;
        QField.QFieldPlaceholderShow(QColophonYear, sheet.CColophonYearFaint);
        QColophonYearSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonYearShown);
        QColophonUrl.Text = sheet.CColophonUrl;
        QField.QFieldPlaceholderShow(QColophonUrl, sheet.CColophonUrlFaint);
        QColophonUrlSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonUrlShown);
        QColophonNote.Text = sheet.CColophonNote;
        QField.QFieldPlaceholderShow(QColophonNote, sheet.CColophonNoteFaint);
        QColophonNoteSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonNoteShown);
        QColophonAuthor.Text = sheet.CColophonAuthor;
        QField.QFieldPlaceholderShow(QColophonAuthor, sheet.CColophonAuthorFaint);
        QColophonAuthorSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonAuthorShown);
        QColophonTally.Text = sheet.CColophonTally;

        QColophonBody.Visibility = Visibility.Visible;
        QColophonUnselected.Visibility = Visibility.Collapsed;
    }

    internal void QColophonTallyShow(string tally)
    {
        QColophonTally.Text = tally;
    }

    internal void QColophonClear()
    {
        QColophonBody.Visibility = Visibility.Collapsed;
        QColophonUnselected.Visibility = Visibility.Visible;
    }
}
