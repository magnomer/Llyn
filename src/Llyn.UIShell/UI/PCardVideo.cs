using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PVideo> PCardVideo { get; } = [];

    internal Action<PVideo, string>? PCardVideoNotice { get; set; }

    internal void PCardVideoShow(IReadOnlyList<LVideoDraft> rows, Func<PVideo, string, bool> pending)
    {
        ArgumentNullException.ThrowIfNull(pending);

        PCardRowShow(
            PCardVideo,
            rows,
            static row => row.PVideoId,
            static draft => draft.LVideoDraftId,
            PCardVideoCreate,
            (row, draft) =>
            {
                row.PVideoShow(draft, field => pending(row, field));
                return row;
            });
    }

    private PVideo PCardVideoCreate(LVideoDraft draft)
    {
        PVideo row = new(draft);
        row.PropertyChanged += PCardVideoChange;
        return row;
    }

    private void PCardVideoChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PVideo row
            && arguments.PropertyName is nameof(PVideo.PVideoLocation) or nameof(PVideo.PVideoTimestamp))
        {
            PCardVideoNotice?.Invoke(row, arguments.PropertyName);
        }
    }
}
