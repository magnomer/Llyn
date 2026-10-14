using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LDepot
{
    Task LDepotSave(string folder, string code, LDeckSheet sheet, bool basic, CancellationToken cancellation);

    LDeckSheet? LDepotRead(string folder, string code);

    string? LDepotDigestRead(string folder, string code);

    string? LDepotMediaResolve(string source);

    bool LDepotMediaCheck(string folder, string code, string name);

    void LDepotMediaCopy(string source, string folder, string code, string name);

    void LDepotMediaDelete(string folder, string code, string name);
}
