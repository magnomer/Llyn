using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

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

    internal void PColophonShow(LColophon sheet)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        PColophonTitle.Text = sheet.LColophonTitle;
        PField.PFieldPlaceholderShow(PColophonTitle, sheet.LColophonTitleFaint);
        PColophonKind.Text = sheet.LColophonKind;
        PColophonChip.Visibility = PLook.PLookVisibleRead(sheet.LColophonKindShown);
        PColophonYear.Text = sheet.LColophonYear;
        PField.PFieldPlaceholderShow(PColophonYear, sheet.LColophonYearFaint);
        PColophonYearSection.Visibility = PLook.PLookVisibleRead(sheet.LColophonYearShown);
        PColophonUrl.Text = sheet.LColophonUrl;
        PField.PFieldPlaceholderShow(PColophonUrl, sheet.LColophonUrlFaint);
        PColophonUrlSection.Visibility = PLook.PLookVisibleRead(sheet.LColophonUrlShown);
        PColophonNote.Text = sheet.LColophonNote;
        PField.PFieldPlaceholderShow(PColophonNote, sheet.LColophonNoteFaint);
        PColophonNoteSection.Visibility = PLook.PLookVisibleRead(sheet.LColophonNoteShown);
        PColophonAuthor.Text = sheet.LColophonAuthor;
        PField.PFieldPlaceholderShow(PColophonAuthor, sheet.LColophonAuthorFaint);
        PColophonAuthorSection.Visibility = PLook.PLookVisibleRead(sheet.LColophonAuthorShown);
        PColophonTally.Text = sheet.LColophonTally;

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
