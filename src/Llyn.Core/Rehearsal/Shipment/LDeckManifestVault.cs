namespace Llyn.Core;

public interface LDeckManifestVault
{
    LDeckManifest LDeckManifestRead();

    void LDeckManifestSave(LDeckManifest manifest);
}
