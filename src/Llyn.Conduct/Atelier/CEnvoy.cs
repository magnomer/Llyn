namespace Llyn.Conduct;

public interface CEnvoy
{
    bool CEnvoyConfirm(string key);

    void CEnvoyFailureShow(string key);
}
