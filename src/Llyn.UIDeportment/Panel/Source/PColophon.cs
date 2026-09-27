using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public class PColophon : UserControl
{
    public PColophon()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Source/PColophon.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
    }

    private StackPanel PColophonBody => (StackPanel)FindName(nameof(PColophonBody));

    private TextBlock PColophonTitle => (TextBlock)FindName(nameof(PColophonTitle));

    private Border PColophonChip => (Border)FindName(nameof(PColophonChip));

    private TextBlock PColophonKind => (TextBlock)FindName(nameof(PColophonKind));

    private TextBlock PColophonTally => (TextBlock)FindName(nameof(PColophonTally));

    private StackPanel PColophonAuthorSection => (StackPanel)FindName(nameof(PColophonAuthorSection));

    private TextBlock PColophonAuthor => (TextBlock)FindName(nameof(PColophonAuthor));

    private StackPanel PColophonYearSection => (StackPanel)FindName(nameof(PColophonYearSection));

    private TextBlock PColophonYear => (TextBlock)FindName(nameof(PColophonYear));

    private StackPanel PColophonUrlSection => (StackPanel)FindName(nameof(PColophonUrlSection));

    private TextBlock PColophonUrl => (TextBlock)FindName(nameof(PColophonUrl));

    private StackPanel PColophonNoteSection => (StackPanel)FindName(nameof(PColophonNoteSection));

    private TextBlock PColophonNote => (TextBlock)FindName(nameof(PColophonNote));

    private TextBlock PColophonUnselected => (TextBlock)FindName(nameof(PColophonUnselected));

    internal void PColophonShow(CColophon sheet)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        PColophonTitle.Text = sheet.CColophonTitle;
        QField.QFieldPlaceholderShow(PColophonTitle, sheet.CColophonTitleFaint);
        PColophonKind.Text = sheet.CColophonKind;
        PColophonChip.Visibility = QLook.QLookVisibleRead(sheet.CColophonKindShown);
        PColophonYear.Text = sheet.CColophonYear;
        QField.QFieldPlaceholderShow(PColophonYear, sheet.CColophonYearFaint);
        PColophonYearSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonYearShown);
        PColophonUrl.Text = sheet.CColophonUrl;
        QField.QFieldPlaceholderShow(PColophonUrl, sheet.CColophonUrlFaint);
        PColophonUrlSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonUrlShown);
        PColophonNote.Text = sheet.CColophonNote;
        QField.QFieldPlaceholderShow(PColophonNote, sheet.CColophonNoteFaint);
        PColophonNoteSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonNoteShown);
        PColophonAuthor.Text = sheet.CColophonAuthor;
        QField.QFieldPlaceholderShow(PColophonAuthor, sheet.CColophonAuthorFaint);
        PColophonAuthorSection.Visibility = QLook.QLookVisibleRead(sheet.CColophonAuthorShown);
        PColophonTally.Text = sheet.CColophonTally;

        PColophonBody.Visibility = Visibility.Visible;
        PColophonUnselected.Visibility = Visibility.Collapsed;
    }

    internal void PColophonTallyShow(string tally)
    {
        PColophonTally.Text = tally;
    }

    internal void PColophonClear()
    {
        PColophonBody.Visibility = Visibility.Collapsed;
        PColophonUnselected.Visibility = Visibility.Visible;
    }
}
