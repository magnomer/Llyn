namespace Llyn.Core;

public interface LMarkupVault
{
    string LMarkupRead(string path);

    void LMarkupSave(string path, string text);
}
