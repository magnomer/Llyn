using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LOutpost
{
    Task<int?> LOutpostFind(int port, CancellationToken cancellation);

    Task<string> LOutpostWarrantStart(int port, CancellationToken cancellation);

    Task<LWarrantAnswer> LOutpostWarrantCheck(int port, string ticket, CancellationToken cancellation);

    Task LOutpostFolderSave(int port, string token, string id, string title, CancellationToken cancellation);

    Task LOutpostNoteSave(int port, string token, LOutpostNote note, CancellationToken cancellation);

    Task<string?> LOutpostNoteRead(
        int port, string token, string id, string folder, string title, CancellationToken cancellation);

    Task<bool> LOutpostNoteRemove(
        int port, string token, string id, string folder, string mark, CancellationToken cancellation);

    Task LOutpostTagSave(
        int port, string token, string id, IReadOnlyList<string> tags, CancellationToken cancellation);

    Task LOutpostParcelSave(int port, string token, LParcel parcel, CancellationToken cancellation);
}
