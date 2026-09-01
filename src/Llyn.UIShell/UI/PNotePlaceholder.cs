using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PEditor
{
    private void PNoteContentsHandle(object sender, TextChangedEventArgs e)
    {
        if (PNotePlaceholder is null || PNoteContents is null)
        {
            return;
        }

        PNotePlaceholder.Visibility = string.IsNullOrWhiteSpace(PNoteContents.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
