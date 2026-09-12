using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PImage> PCardImage { get; } = [];

    internal Action<PImage>? PCardImageNotice { get; set; }

    internal void PCardImageShow(IReadOnlyList<LImageDraft> rows, Func<PImage, bool> pending)
    {
        ArgumentNullException.ThrowIfNull(pending);

        PCardRowShow(
            PCardImage,
            rows,
            static row => row.PImageId,
            static draft => draft.LImageDraftId,
            PCardImageCreate,
            (row, draft) =>
            {
                row.PImageShow(draft, pending(row));
                return row;
            });
    }

    private PImage PCardImageCreate(LImageDraft draft)
    {
        PImage row = new(draft);
        row.PropertyChanged += PCardImageChange;
        return row;
    }

    private void PCardImageChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PImage row
            && string.Equals(arguments.PropertyName, nameof(PImage.PImageLocation), StringComparison.Ordinal))
        {
            PCardImageNotice?.Invoke(row);
        }
    }
}
