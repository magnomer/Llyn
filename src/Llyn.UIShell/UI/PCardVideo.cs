using System.Collections.ObjectModel;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PVideo> PCardVideo { get; }

    internal void PCardVideoAdd()
    {
        PCardVideo.Add(new PVideo());
    }

    internal void PCardVideoRemove(PVideo row)
    {
        PCardVideo.Remove(row);
    }
}
