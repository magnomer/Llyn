using System.Collections.Generic;

namespace Llyn.Conduct;

public interface CEnvoy
{
    bool CEnvoyConfirm(string key);

    bool CEnvoyConfirm(string key, string tallyKey, int tally);

    bool CEnvoyUnionConfirm(string key, string dropped, string kept);

    void CEnvoyFailureShow(string key);

    void CEnvoyFailureShow(string key, CLedgerNotice notice);

    bool? CEnvoyLeaveConfirm();

    IReadOnlyList<CSCustomsRow>? CEnvoyCustomsRead(IReadOnlyList<CMarkupEntry> entries);

    void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions);

    (string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(
        string file, IReadOnlyList<CPortraitChoice> choices);

    CPressTicket? CEnvoyTicketRead();

    string? CEnvoyWorkspaceRead(string workspace);
}
