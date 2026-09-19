namespace Llyn.Core;

public interface LKeep
{
    string? LKeepRead(string name);

    void LKeepSave(string name, string text);
}
