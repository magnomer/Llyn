namespace Llyn.Core;

public interface LMarkupVault
{
    LMarkupNode LMarkupRead(string path);

    void LMarkupSave(string path, LMarkupNode root);
}
