using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

/// <summary>
/// The prompt shown over an empty note. WPF text boxes carry no placeholder of their own, so the
/// hint is a separate element whose visibility follows whether anything has been typed.
/// </summary>
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
