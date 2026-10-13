using System.Collections.ObjectModel;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardRow
{
    [Fact]
    public void CardRowShow_DuplicateIds_KeepsEachRowOnce()
    {
        QTranscriptionItem first = new(5, "Alpha", "one");
        QTranscriptionItem second = new(5, "Alpha", "two");
        ObservableCollection<QTranscriptionItem> rows = [first, second];

        TInterfaceDeportment.TCardRowShow(
            rows,
            [new CTranscriptionDraft(5, "Alpha", "uno"), new(5, "Alpha", "dos"), new(5, "Alpha", "tres")]);

        Assert.Equal(3, rows.Count);
        Assert.Same(first, rows[0]);
        Assert.Same(second, rows[1]);
        Assert.NotSame(first, rows[2]);
        Assert.NotSame(second, rows[2]);
        Assert.Equal("uno", first.QTranscriptionItemText);
        Assert.Equal("dos", second.QTranscriptionItemText);
        Assert.Equal("tres", rows[2].QTranscriptionItemText);
    }
}
