namespace Llyn.Core;

public interface LManifestVault
{
    LManifest LManifestRead();

    void LManifestSave(LManifest manifest);
}
