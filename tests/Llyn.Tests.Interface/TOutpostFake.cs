using Llyn.Core;

namespace Llyn.Tests;

public sealed class TOutpostFake : LOutpost
{
    private const int TOutpostFakePort = 41184;

    public Dictionary<string, (string TOutpostFakeParent, string TOutpostFakeTitle)> TOutpostFakeFolder { get; } = [];

    public Dictionary<string, LOutpostNote> TOutpostFakeNote { get; } = [];

    public List<string> TOutpostFakeSaved { get; } = [];

    public List<string> TOutpostFakeTrash { get; } = [];

    public HashSet<string> TOutpostFakeRefusal { get; } = new(StringComparer.Ordinal);

    public Task<int?> LOutpostFind(int port, CancellationToken cancellation) =>
        Task.FromResult<int?>(TOutpostFakePort);

    public Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation) =>
        throw new NotSupportedException();

    public Task<LWarrantAnswer> LOutpostWarrantCheck(int port, string ticket, CancellationToken cancellation) =>
        throw new NotSupportedException();

    public Task LOutpostFolderSave(
        int port, string token, string id, string parent, string title, CancellationToken cancellation)
    {
        TOutpostFakeFolder[id] = (parent, title);
        return Task.CompletedTask;
    }

    public Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation)
    {
        if (TOutpostFakeRefusal.Contains(note.LOutpostNoteTitle))
        {
            throw new InvalidOperationException("Joplin refused the note.");
        }

        TOutpostFakeNote[note.LOutpostNoteId] = note;
        TOutpostFakeSaved.Add(note.LOutpostNoteId);
        return Task.CompletedTask;
    }

    public Task<string?> LOutpostNoteRead(
        int port, string token, string id, string folder, string title, CancellationToken cancellation) =>
        Task.FromResult(TOutpostFakeNote.GetValueOrDefault(id)?.LOutpostNoteBody);

    public Task<bool> LOutpostNoteRemove(
        int port, string token, string id, IReadOnlySet<string> folders, string mark, CancellationToken cancellation)
    {
        TOutpostFakeTrash.Add(id);
        if (!TOutpostFakeNote.TryGetValue(id, out LOutpostNote? note)
            || !folders.Contains(note.LOutpostNoteFolder)
            || !note.LOutpostNoteBody.StartsWith(mark, StringComparison.Ordinal))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(TOutpostFakeNote.Remove(id));
    }

    public Task LOutpostTagSave(
        int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation) =>
        Task.CompletedTask;

    public Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation) =>
        Task.CompletedTask;
}
