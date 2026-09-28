using System;

namespace Llyn.Conduct;

public interface CEnvoy
{
    bool CEnvoyConfirm(string key);

    bool CEnvoyConfirm(string key, string tallyKey, int tally);

    bool CEnvoyUnionConfirm(string key, string dropped, string kept);

    void CEnvoyFailureShow(string key);

    void CEnvoyFailureShow(string key, Exception exception);

    bool CEnvoyDiscardConfirm();

    bool? CEnvoyLeaveConfirm();
}
