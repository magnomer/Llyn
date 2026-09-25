using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PColophon : UserControl
{
    public PColophon()
    {
        InitializeComponent();
    }

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
